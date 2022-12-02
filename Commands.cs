using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

namespace GenGcode
{
   public class Commands : IExtensionApplication
   {
      List<string> _totalGCode = new List<string>();
      // функция инициализации (выполняется при загрузке плагина)
      public void Initialize()
      {
         MessageBox.Show("GenGcode loaded. \r\n Type \"GETGCODE\" to run \r\n  or \"SETGCODE\" to setup");
         // получаем текущую БД
      }

      // эта функция будет вызываться при выполнении в AutoCAD команды «TestCommand»
      [CommandMethod("GETGCODE")]
      public void GetGcode()
      {
         List<string> gcode = new List<string>();
         int speed = 1000;
         int power = 1000;
         int repeat = 1;

         Database db = HostApplicationServices.WorkingDatabase;
         Dictionary<string, string> props = ReadCustomProp(db);

         // начинаем транзакцию
         using (Transaction tr = db.TransactionManager.StartTransaction())
         {
            // получаем ссылку на пространство модели (ModelSpace)
            BlockTableRecord ms = (BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db), OpenMode.ForRead);

            foreach (ObjectId id in ms)
            {
               List<string> output = new List<string>();

               Entity entity = (Entity)tr.GetObject(id, OpenMode.ForRead);

               if (!props.ContainsKey(entity.Layer))
               {
                  SetGcode();
                  props = ReadCustomProp(db);
               }

               ReadLayerParams(out speed, out power, out repeat, props, entity.Layer);

               if (entity.GetType() == typeof(Polyline))
               {
                  Polyline polyline = entity as Polyline;
                  output = GetGcode(polyline, speed, power* 10);
               }

               if (entity.GetType() == typeof(Circle))
               {
                  Circle circle = entity as Circle;
                  output = GetGcode(circle, speed , power* 10);
               }

               for (int i = 0; i < repeat; i++)
               {
                  gcode.AddRange(output);
               }
            }

            //Walk perimeter

            _totalGCode.Add($"G0X{Gcode.minX}Y{Gcode.minY}");
            _totalGCode.Add($"G0X{Gcode.minX}Y{Gcode.maxY}");
            _totalGCode.Add($"G0X{Gcode.maxX}Y{Gcode.maxY}");
            _totalGCode.Add($"G0X{Gcode.maxX}Y{Gcode.minY}");
            _totalGCode.AddRange(gcode);

            tr.Commit();
         }

         SaveGcode(_totalGCode);
      }

      public static bool ReadLayerParams(out int speed, out int power, out int repeat, Dictionary<string, string> props, string layerName)
      {
         if (props.ContainsKey(layerName))
         {
            var values = props[layerName].Split(';');
            speed = Convert.ToInt32(values[0]);
            power = Convert.ToInt32(values[1]);
            repeat = Convert.ToInt32(values[2]);
            return true;
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

  
      public static Dictionary<string, string> ReadCustomProp(Database db)
      {
         var records = db.SummaryInfo.CustomProperties;

         Dictionary<string, string> custumProp = new Dictionary<string, string>();

         while (records.MoveNext())
         {
            custumProp.Add(records.Key.ToString(), records.Value.ToString());
         }

         return custumProp;
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

      private static void SaveGcode(List<string> gcode)
      {
         Document doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
         string drawingName = System.IO.Path.GetFileName(doc.Name).Split('.')[0];

         SaveFileDialog saveFileDialog = new SaveFileDialog();
         saveFileDialog.Filter = "GCODE files(*.nc)|*.nc|All files(*.*)|*.*";

         saveFileDialog.FileName = drawingName + ".nc";
         if (saveFileDialog.ShowDialog() == DialogResult.Cancel)
            return;

         // получаем выбранный файл
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

      // функция, выполняемая при выгрузке плагина
      public void Terminate()
      {
      }
   }
}