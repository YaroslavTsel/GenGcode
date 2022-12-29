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
     public static bool _passByEntity;
      public void Initialize()
      {
         MessageBox.Show("GenGcode loaded. \r\n Type \"GETGCODE\" to run \r\n  or \"SETGCODE\" to setup");

      }

      [CommandMethod("GETGCODE", CommandFlags.UsePickSet)]

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

            var entryByLayerDict = EntryByLayer(tr, elements);

            props = ChkContainsLayer(db, props, entryByLayerDict);

            foreach (var prop in props)
            {
               string layerName = prop.Value.Split(';')[0];

               if (!entryByLayerDict.ContainsKey(layerName)) continue;


               var polylines = new List<Polyline>();
               var circles = new List<Circle>();

               GetPolylinesAndCircles(entryByLayerDict, layerName, polylines, circles);

               List<Polyline> optimizedPolylines = GetOptimizedPolylines(ref polylines, tr);

               List<Circle> optimtzedCircles = GetOpitmtzedCircles(ref circles);

               ReadLayerParams(out speed, out power, out repeat, prop.Value);

               GetLayerGcode(gcode, speed, power, repeat, layerName, optimizedPolylines, optimtzedCircles, _passByEntity, tr);

            }

            AddStartGcode();

            _totalGCode.AddRange(gcode);

            tr.Commit();
         }

         if (!(Gcode.OutRange && MessageBox.Show($"Your drawing is below 0 coordinates. OK?", "Attention!", MessageBoxButtons.OKCancel) == DialogResult.Cancel))
         {
            SaveGcode(_totalGCode, selected);
         }


         void AddStartGcode()
         {
            _totalGCode.Add(";This gcode was genrated by GenGcode Autocad plugin");
            _totalGCode.Add(";Latest version of the plugin you can get on https://github.com/YaroslavTsel/GenGcode");

            //Walk perimeter

            _totalGCode.Add($"G0X{Gcode.minX}Y{Gcode.minY}");
            _totalGCode.Add($"G0X{Gcode.minX}Y{Gcode.maxY}");
            _totalGCode.Add($"G0X{Gcode.maxX}Y{Gcode.maxY}");
            _totalGCode.Add($"G0X{Gcode.maxX}Y{Gcode.minY}");
         }
      }

      private static Dictionary<string, List<Entity>> EntryByLayer(Transaction tr, List<ObjectId> elements)
      {
         List<Entity> entities = new List<Entity>();

         foreach (ObjectId id in elements)
         {
            entities.Add((Entity)tr.GetObject(id, OpenMode.ForRead));

         }
         return entities.GroupBy(o => o.Layer).ToDictionary(g => g.Key, g => g.ToList());
      }

      private SortedDictionary<string, string> ChkContainsLayer(Database db, SortedDictionary<string, string> props, Dictionary<string, List<Entity>> entryByLayerDict)
      {
         foreach (var layer in entryByLayerDict.Keys)
         {
            List<string> propLayers = props.Values.Select(x => x.Split(';')[0]).ToList();

            if (!propLayers.Contains(layer))
            {
               SetGcode();
               props = ReadCustomProp(db);
            }
         }

         return props;
      }

      private void GetPolylinesAndCircles(Dictionary<string, List<Entity>> entryByLayerDict, string layerName, List<Polyline> polylines, List<Circle> circles)
      {
         foreach (Entity entity in entryByLayerDict[layerName])
         {

            if (entity.GetType() == typeof(Polyline))
            {
               Polyline polyline = entity as Polyline;

               polylines.Add(polyline);

            }
            if (entity.GetType() == typeof(Circle))
            {
               Circle circle = entity as Circle;
               circles.Add(circle);
            }

         }
      }

      private static List<Polyline> GetOptimizedPolylines(ref List<Polyline> polylines, Transaction tr)
      {
         Point3d point = new Point3d(0, 0, 0);







         List<Polyline> result = new List<Polyline>();
         while (polylines.Count > 0)
         {

            double minStartDist = polylines.Min(x => x.StartPoint.DistanceTo(point));
            double minEndDist = polylines.Min(x => x.EndPoint.DistanceTo(point));

            var shortestStartPoly = polylines.FirstOrDefault(x => x.StartPoint.DistanceTo(point) == minStartDist);
            var shortestEndPoly = polylines.FirstOrDefault(x => x.EndPoint.DistanceTo(point) == minEndDist);


            //var shortestStartPoly = polylines.OrderBy(x => x.StartPoint.DistanceTo(point)).FirstOrDefault();
            //var shortestEndPoly = polylines.OrderBy(x => x.EndPoint.DistanceTo(point)).FirstOrDefault();

            if (shortestStartPoly.StartPoint.DistanceTo(point) < shortestEndPoly.EndPoint.DistanceTo(point))
            {
               result.Add(shortestStartPoly);
               polylines = polylines.Where(x => !x.Equals(shortestStartPoly)).ToList();
               point = shortestStartPoly.EndPoint;
            }
            else
            {
               polylines = polylines.Where(x => !x.Equals(shortestEndPoly)).ToList();

               var poly = tr.GetObject(shortestEndPoly.Id, OpenMode.ForWrite) as Polyline;
               poly.ReverseCurve();
               result.Add(shortestEndPoly);
               point = shortestEndPoly.EndPoint;

            }
         }

         return result;
      }

      private static List<Circle> GetOpitmtzedCircles(ref List<Circle> circles)
      {
         var optimtzedCircles = new List<Circle>();
         Point3d point = new Point3d(0, 0, 0);


         while (circles.Count > 0)
         {
            var shortestCircle = circles.OrderBy(x => x.StartPoint.DistanceTo(point)).FirstOrDefault();

            optimtzedCircles.Add(shortestCircle);
            circles = circles.Where(x => !x.Equals(shortestCircle)).ToList();
            point = shortestCircle.EndPoint;

         }

         return optimtzedCircles;
      }

      private static void GetLayerGcode(List<string> gcode, int speed, int power, int repeat, string layerName,
                                        List<Polyline> optimizedPolylines, List<Circle> optimtzedCircles, bool repeatEachEntity, Transaction tr)
      {

         List<string> output = new List<string>();

         if (repeatEachEntity)
         {
            foreach (var polyline in optimizedPolylines)
            {
               for (int i = 0; i < repeat; i++)
               {
                  output.Add($";Polyline ID: {polyline.Id}, Layer  {layerName}, pass {i+1}");
                  output.AddRange(GetGcode(polyline, speed, power * 10));
                  var poly = tr.GetObject(polyline.Id, OpenMode.ForWrite) as Polyline;
                  poly.ReverseCurve();
                  
               }
            }

            foreach (var circle in optimtzedCircles)
            {
               for (int i = 0; i < repeat; i++)
               {
                  output.Add($";Circle ID: {circle.Id}, Layer  {layerName}, pass {i+1}");
                  output.AddRange(GetGcode(circle, speed, power * 10));

               }
            }

            gcode.AddRange(output);

         }
         else
         {
            for (int i = 0; i < repeat; i++)
            {

               foreach (var polyline in optimizedPolylines)
               {

                  output.AddRange(GetGcode(polyline, speed, power * 10));
               }

               foreach (var circle in optimtzedCircles)
               {
                  output.AddRange(GetGcode(circle, speed, power * 10));
               }


               gcode.Add($";Layer  {layerName}, pass {i + 1}");
               gcode.AddRange(output);
            }
         }
      }




      private static List<ObjectId> GetSelectedElements()
      {
         Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
         var sel = ed.SelectImplied().Value;
         return sel?.GetObjectIds()?.ToList();
      }

      public static bool ReadLayerParams(out int speed, out int power, out int repeat, string prop)
      {

         var values = prop.Split(';');


         speed = Convert.ToInt32(values[1]);
         power = Convert.ToInt32(values[2]);
         repeat = Convert.ToInt32(values[3]);
         return true;

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
            if (records.Key.ToString().Contains("EngraveProp"))
               customProp.Add(records.Key.ToString(), records.Value.ToString());
            else if (records.Key.ToString() == "PassByElement")
            {
               _passByEntity = Convert.ToBoolean(records.Value);
            }
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

               polyGCode.addLine(polyline, i);
            }
            if (segmentType == SegmentType.Arc)
            {

               polyGCode.addArc(polyline, i);
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