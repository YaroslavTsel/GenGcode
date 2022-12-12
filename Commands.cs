using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.EditorInput;

namespace GenGcode
{
   public class Commands : IExtensionApplication
   {
      List<string> _totalGCode;

      public void Initialize()
      {
         MessageBox.Show("GenGcode loaded. \r\n Type \"GETGCODE\" to run \r\n  or \"SETGCODE\" to setup");

      }

      [CommandMethod("GETGCODE",CommandFlags.UsePickSet)]

      public void GetGcode()
      {
         _totalGCode = new List<string>();
         Gcode.maxX = 0;
         Gcode.maxY = 0;
         Gcode.minX = 0;
         Gcode.minY = 0;
         bool selected = true;
         List<string> gcode = new List<string>();
         int speed = 1000;
         int power = 1000;
         int repeat = 1;

         Database db = HostApplicationServices.WorkingDatabase;
         SortedDictionary<string, string> props = ReadCustomProp(db);

         List<ObjectId> elements = GetSelectedElements();

         using (Transaction tr = db.TransactionManager.StartTransaction())
         {
            if (elements == null)
            {
               elements = new List<ObjectId>();
               selected = false;
               BlockTableRecord ms = (BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db), OpenMode.ForRead);
               foreach (ObjectId id in ms)
               {
                  elements.Add(id);
               }
            }

           var entryByLayerDict =  EntryByLayer(tr);

            foreach (var prop in props)
            {

              string  layerName = prop.Value.Split(';')[0];

               foreach (Entity entity in entryByLayerDict[layerName])
               {
                  List<string> output = new List<string>();

                  if (!ReadLayerParams(out speed, out power, out repeat, props, entity.Layer))
                  {
                     SetGcode();
                     props = ReadCustomProp(db);
                  }
                                   

                  if (entity.GetType() == typeof(Polyline))
                  {
                     Polyline polyline = entity as Polyline;
                     output = GetGcode(polyline, speed, power * 10);
                  }

                  if (entity.GetType() == typeof(Circle))
                  {
                     Circle circle = entity as Circle;
                     output = GetGcode(circle, speed, power * 10);
                  }

                  for (int i = 0; i < repeat; i++)
                  {
                     gcode.Add($";{entity.GetType().ToString().Split('.').Last()} on layer  {entity.Layer}, pass {i + 1}");
                     gcode.AddRange(output);
                  }
               }

            }
            _totalGCode.Add(";This gcode was genrated by GenGcode Autocad plugin");
            _totalGCode.Add(";Latest version of the plugin you can get on https://github.com/YaroslavTsel/GenGcode");

            //Walk perimeter

            _totalGCode.Add($"G0X{Gcode.minX}Y{Gcode.minY}");
            _totalGCode.Add($"G0X{Gcode.minX}Y{Gcode.maxY}");
            _totalGCode.Add($"G0X{Gcode.maxX}Y{Gcode.maxY}");
            _totalGCode.Add($"G0X{Gcode.maxX}Y{Gcode.minY}");

            _totalGCode.AddRange(gcode);

            tr.Commit();
         }

         SaveGcode(_totalGCode,selected);

         Dictionary<string, List<Entity>> EntryByLayer(Transaction tr)
         {
            List<Entity> entities = new List<Entity>();

            foreach (ObjectId id in elements)
            {
               entities.Add((Entity)tr.GetObject(id, OpenMode.ForRead));

            }
             return entities.GroupBy(o => o.Layer).ToDictionary(g => g.Key, g => g.ToList());
         }
      }


      private static List<ObjectId> GetSelectedElements()
      {
         Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
         var  sel = ed.SelectImplied().Value;
         return sel?.GetObjectIds()?.ToList();
      }

      public static bool ReadLayerParams( out int speed, out int power, out int repeat, SortedDictionary<string, string> props,string layerName)
      {
         foreach (var prop in props)
         {
            var values = prop.Value.Split(';');

            if (layerName == values[0])
            {
               speed = Convert.ToInt32(values[1]);
               power = Convert.ToInt32(values[2]);
               repeat = Convert.ToInt32(values[3]);
               return true;
            } 
         }
         speed = 0;
         power = 0;
         repeat =0;
         return false;
      }

      [CommandMethod("SETGCODE")]
      public void SetGcode()
      {
         Database db = HostApplicationServices.WorkingDatabase;
         using (GenGcode.SettingsForm mform1 = new GenGcode.SettingsForm())
         {
            mform1.ShowDialog();
         }
      }

  
      public static SortedDictionary<string, string> ReadCustomProp(Database db)
      {
         var records = db.SummaryInfo.CustomProperties;

         SortedDictionary<string, string> customProp = new SortedDictionary<string, string>();

         while (records.MoveNext())
         {
            customProp.Add(records.Key.ToString(), records.Value.ToString());
         }

         return customProp;
      }

      private static List<string> GetGcode(Polyline polyline, int speed, int power)
      {
         Gcode polyGCode = new Gcode(speed, power);

         int segmentsCount = polyline.Closed ? polyline.NumberOfVertices : polyline.NumberOfVertices - 1;

         for (int i = 0; i < segmentsCount; i++)
         {
            SegmentType segmentType = polyline.GetSegmentType(i);

            if (segmentType == SegmentType.Line)
            {
               LineSegment2d line = polyline.GetLineSegment2dAt(i);
               polyGCode.addLine(line);
            }
            if (segmentType == SegmentType.Arc)
            {
               CircularArc2d arc = polyline.GetArcSegment2dAt(i);
               polyGCode.addArc(arc);
            }
         }

         return polyGCode.OutGcode;
      }

      private static List<string> GetGcode(Circle circle, int speed, int power)
      {
         Gcode circleGcode = new Gcode(speed, power);

         circleGcode.addCircle(circle);

         return circleGcode.OutGcode;
      }

      private static void SaveGcode(List<string> gcode, bool selected)
      {
         string suffix = selected ? "_Selected" : "_All";
         Document doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
         string drawingName = System.IO.Path.GetFileName(doc.Name).Split('.')[0];
         
         SaveFileDialog saveFileDialog = new SaveFileDialog();
         saveFileDialog.Filter = "GCODE files(*.nc)|*.nc|All files(*.*)|*.*";

         saveFileDialog.FileName = drawingName + suffix + ".nc";
         if (saveFileDialog.ShowDialog() == DialogResult.Cancel)
            return;

         string filename = saveFileDialog.FileName;

         FileStream fs = new FileStream(@filename, FileMode.Create);
         StreamWriter streamWriter = new StreamWriter(fs);
         try
         {
            foreach (String str in gcode)
            {
               streamWriter.Write(str);
               streamWriter.WriteLine();
            }

            streamWriter.Close();
            fs.Close();
         }
         catch
         {
            MessageBox.Show(@"Ошибка при сохранении файла!");
         }
      }

      public void Terminate()
      {
      }
   }
}