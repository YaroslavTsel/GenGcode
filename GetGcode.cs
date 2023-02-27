using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

namespace GenGcode
{
   public class GetGcode
   {
      private List<string> _totalGCode;
      public static bool _passByEntity;
      public GetGcode()
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
         bool dynPwr = false;

         Database db = HostApplicationServices.WorkingDatabase;
         SortedDictionary<string, string> props = Commands.ReadCustomProp(db);

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
               var lines = new List<Line>();
               var circles = new List<Circle>();
               var arcs = new List<Arc>();

               GetPolylinesAndCircles(entryByLayerDict, layerName, polylines, circles, lines, arcs);

               List<Polyline> optimizedPolylines = GetOptimizedPolylines(ref polylines, tr);

               List<Line> optimizedLines = GetOptimizedLines(ref lines, tr);

               List<Arc> optimizedArcs = GetOptimizedArcs(ref arcs, tr);

               List<Circle> optimtzedCircles = GetOpitmtzedCircles(ref circles);

               ReadLayerParams(out speed, out power, out repeat,out dynPwr,  prop.Value);

               GetLayerGcode(gcode, speed, power, repeat, layerName,dynPwr, optimizedPolylines,
                           optimizedLines, optimizedArcs, optimtzedCircles,
                           _passByEntity, tr);
            }

            AddStartGcode();

            _totalGCode.AddRange(gcode);
            _totalGCode.Add("G0 X0 Y0");
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

            _totalGCode.Add($"G0 X{Gcode.minX} Y{Gcode.minY}");
            _totalGCode.Add($"G0 X{Gcode.minX} Y{Gcode.maxY}");
            _totalGCode.Add($"G0 X{Gcode.maxX} Y{Gcode.maxY}");
            _totalGCode.Add($"G0 X{Gcode.maxX} Y{Gcode.minY}");
         }
      }

      private static Dictionary<string, List<Entity>> EntryByLayer(Transaction tr, List<ObjectId> elements)
      {
         List<Entity> entities = new List<Entity>();

         foreach (ObjectId id in elements)
         {
            entities.Add((Entity)tr.GetObject(id, OpenMode.ForRead));
         }
         return entities.Where(x => x.GetType() == typeof(Polyline)
                              || x.GetType() == typeof(Line)
                              || x.GetType() == typeof(Circle)
                              || x.GetType() == typeof(Arc))
                        .GroupBy(o => o.Layer)
                        .ToDictionary(g => g.Key, g => g.ToList());
      }

      private SortedDictionary<string, string> ChkContainsLayer(Database db, SortedDictionary<string, string> props, Dictionary<string, List<Entity>> entryByLayerDict)
      {
         foreach (var layer in entryByLayerDict.Keys)
         {
            List<string> propLayers = props.Values.Select(x => x.Split(';')[0]).ToList();

            if (!propLayers.Contains(layer))
            {
               using (GenGcode.SettingsForm setGcodeForm = new GenGcode.SettingsForm())
               {
                  setGcodeForm.ShowDialog();
               }
               props = Commands.ReadCustomProp(db);
            }
         }

         return props;
      }

      private void GetPolylinesAndCircles(Dictionary<string, List<Entity>> entryByLayerDict, string layerName, List<Polyline> polylines,
                                                                                                                List<Circle> circles,
                                                                                                                List<Line> lines,
                                                                                                                List<Arc> arcs)
      {
         foreach (Entity entity in entryByLayerDict[layerName])
         {
            //if (entity.GetType() == typeof(Polyline2d))
            //{
            //   Polyline polyline = new Polyline();

            //      Polyline2d poly2d = tr.GetObject(entity.ObjectId, OpenMode.ForWrite) as Polyline2d;

            //      polyline.ConvertFrom(poly2d, true);

            //      polylines.Add(polyline);

            //}

            if (entity.GetType() == typeof(Line))
            {
               lines.Add(entity as Line);
            }

            if (entity.GetType() == typeof(Arc))
            {
               arcs.Add(entity as Arc);
            }

            if (entity.GetType() == typeof(Polyline))
            {
               polylines.Add(entity as Polyline);
            }

            if (entity.GetType() == typeof(Circle))
            {
               circles.Add(entity as Circle);
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

      private List<Line> GetOptimizedLines(ref List<Line> lines, Transaction tr)
      {
         Point3d point = new Point3d(0, 0, 0);

         List<Line> result = new List<Line>();

         while (lines.Count > 0)
         {
            double minStartDist = lines.Min(x => x.StartPoint.DistanceTo(point));
            double minEndDist = lines.Min(x => x.EndPoint.DistanceTo(point));

            var shortestStart = lines.FirstOrDefault(x => x.StartPoint.DistanceTo(point) == minStartDist);
            var shortestEnd = lines.FirstOrDefault(x => x.EndPoint.DistanceTo(point) == minEndDist);

            if (shortestStart.StartPoint.DistanceTo(point) < shortestEnd.EndPoint.DistanceTo(point))
            {
               result.Add(shortestStart);
               lines = lines.Where(x => !x.Equals(shortestStart)).ToList();
               point = shortestStart.EndPoint;
            }
            else
            {
               lines = lines.Where(x => !x.Equals(shortestEnd)).ToList();

               var line = tr.GetObject(shortestEnd.Id, OpenMode.ForWrite) as Line;
               line.ReverseCurve();
               result.Add(shortestEnd);
               point = shortestEnd.EndPoint;
            }
         }

         return result;
      }

      private List<Arc> GetOptimizedArcs(ref List<Arc> arcs, Transaction tr)
      {
         var optimtzedArcs = new List<Arc>();
         Point3d point = new Point3d(0, 0, 0);

         while (arcs.Count > 0)
         {
            var shortestArc = arcs.OrderBy(x => x.StartPoint.DistanceTo(point)).FirstOrDefault();

            optimtzedArcs.Add(shortestArc);
            arcs = arcs.Where(x => !x.Equals(shortestArc)).ToList();
            point = shortestArc.EndPoint;
         }

         return optimtzedArcs;
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

      private static void GetLayerGcode(List<string> gcode, int speed, int power, int repeat, string layerName,bool dynPwr, 
                                        List<Polyline> optimizedPolylines, List<Line> optimizedlines, List<Arc> optimizedArcs,
                                        List<Circle> optimtzedCircles, bool repeatEachEntity,Transaction tr)
      {
         List<string> output = new List<string>();

         if (repeatEachEntity)
         {
            foreach (var polyline in optimizedPolylines)
            {
               for (int i = 0; i < repeat; i++)
               {
                  output.Add($";Polyline ID: {polyline.Id}, Layer  {layerName}, pass {i + 1}");
                  output.AddRange(GetEntityGcode(polyline, speed, power * 10,dynPwr));
                  var poly = tr.GetObject(polyline.Id, OpenMode.ForWrite) as Polyline;
                  poly.ReverseCurve();
               }
            }

            foreach (var line in optimizedlines)
            {
               for (int i = 0; i < repeat; i++)
               {
                  output.Add($";Line ID: {line.Id}, Layer  {layerName}, pass {i + 1}");
                  output.AddRange(GetEntityGcode(line, speed, power * 10,dynPwr));
                  var gotline = tr.GetObject(line.Id, OpenMode.ForWrite) as Line;
                  gotline.ReverseCurve();
               }
            }


            foreach (var arc in optimizedArcs)
            {
               for (int i = 0; i < repeat; i++)
               {
                  output.Add($";Circle ID: {arc.Id}, Layer  {layerName}, pass {i + 1}");
                  output.AddRange(GetEntityGcode(arc, speed, power * 10,dynPwr));
                  var gotArc = tr.GetObject(arc.Id, OpenMode.ForWrite) as Arc;
                  gotArc.ReverseCurve();
               }
            }

            foreach (var circle in optimtzedCircles)
            {
               for (int i = 0; i < repeat; i++)
               {
                  output.Add($";Circle ID: {circle.Id}, Layer  {layerName}, pass {i + 1}");
                  output.AddRange(GetEntityGcode(circle, speed, power * 10,dynPwr));
               }
            }

            foreach (var circle in optimtzedCircles)
            {
               for (int i = 0; i < repeat; i++)
               {
                  output.Add($";Circle ID: {circle.Id}, Layer  {layerName}, pass {i + 1}");
                  output.AddRange(GetEntityGcode(circle, speed, power * 10,dynPwr));
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
                  output.Add($";Polyline ID: {polyline.Id}, Layer  {layerName}, pass {i + 1}");
                  output.AddRange(GetEntityGcode(polyline, speed, power * 10,dynPwr));
               }

               foreach (var line in optimizedlines)
               {
                  output.Add($";Line ID: {line.Id}, Layer  {layerName}, pass {i + 1}");
                  output.AddRange(GetEntityGcode(line, speed, power * 10,dynPwr));

               }

               foreach (var arc in optimizedArcs)
               {
                  output.Add($";Line ID: {arc.Id}, Layer  {layerName}, pass {i + 1}");
                  output.AddRange(GetEntityGcode(arc, speed, power * 10,dynPwr));

               }


               foreach (var circle in optimtzedCircles)
               {
                  output.Add($";Circle ID: {circle.Id}, Layer  {layerName}, pass {i + 1}");
                  output.AddRange(GetEntityGcode(circle, speed, power * 10, dynPwr));
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

      public static bool ReadLayerParams(out int speed, out int power, out int repeat,out bool dynPwr, string prop)
      {
         var values = prop.Split(';');

         speed = Convert.ToInt32(values[1]);
         power = Convert.ToInt32(values[2]);
         repeat = Convert.ToInt32(values[3]);
         dynPwr = Convert.ToBoolean(values[4]);
         return true;
      }

      private static void SaveGcode(List<string> gcode, bool selected)
      {
         string suffix = selected ? "_Selected" : "_All";
         Document doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
         string drawingName = System.IO.Path.GetFileName(doc.Name).Split('.')[0];

         string outFileName = "";

         foreach (char letter in drawingName)
         {
            if (((int)letter < 128 && (int)letter > 175) || ((int)letter < 224 && (int)letter > 240))
            {
               outFileName += letter;
            }
            else
            {
               outFileName += 'x';
            }
         }

         SaveFileDialog saveFileDialog = new SaveFileDialog();
         saveFileDialog.Filter = "GCODE files(*.nc)|*.nc|All files(*.*)|*.*";

         saveFileDialog.FileName = outFileName + suffix + ".nc";
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
      private static List<string> GetEntityGcode(Polyline polyline, int speed, int power, bool dynPwr)
      {
         Gcode polyGCode = new Gcode(speed, power,dynPwr);

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
      private static List<string> GetEntityGcode(Line line, int speed, int power, bool dynPwr)
      {
         Gcode polyGCode = new Gcode(speed, power,dynPwr);
         polyGCode.addLine(line);
         return polyGCode.OutGcode;
      }


      private static List<string> GetEntityGcode(Arc arc, int speed, int power,bool dynPwr)
      {
         Gcode polyGCode = new Gcode(speed, power, dynPwr);
         polyGCode.addArc(arc);
         return polyGCode.OutGcode;
      }



      private static List<string> GetEntityGcode(Circle circle, int speed, int power,bool dynPwr)
      {
         Gcode circleGcode = new Gcode(speed, power, dynPwr);

         circleGcode.addCircle(circle);

         return circleGcode.OutGcode;
      }

   }
}