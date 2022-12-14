using System;
using System.Collections.Generic;

using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

using Source;

namespace GenGcode
{
   internal class Gcode
   {
      private List<string> _outGCode = new List<string>();
      private bool _first = true;
      private int _speed, _power;
      
      private bool _waring = false;

      public static double minX, minY, maxX, maxY;




      public static bool OutRange
      {
         get
         {
            if(minX<0||minY<0||maxX<0||maxY<0)
               return true;
            else 
               return false;
         }
      }

      public List<string> OutGcode
      {
         get
         {
            _outGCode.Add("M5 S0");
            return _outGCode;
         }
      }

      public Gcode(int speed, int power)
      {
         _speed = speed;
         _power = power; 
         _outGCode.Add("M3 S0");

         
      }

      public void addLine(Polyline polyline, int index)
      {

         LineSegment2d line2d = polyline.GetLineSegment2dAt(index);

         LineSegment3d line3d = polyline.GetLineSegmentAt(index);
        


         if (Math.Round(line3d.StartPoint.X, 3) != Math.Round(line2d.StartPoint.X, 3) ||
           Math.Round(line3d.EndPoint.X, 3) != Math.Round(line2d.EndPoint.X, 3) ||
           Math.Round(line3d.StartPoint.Y, 3) != Math.Round(line2d.StartPoint.Y, 3) ||
           Math.Round(line3d.EndPoint.Y, 3) != Math.Round(line2d.EndPoint.Y, 3))

         {
            Source.ElemsLib.HighlightElements(new ObjectId[1] { polyline.Id});

            if (!_waring)
            {
               System.Windows.Forms.MessageBox.Show($"Line segment error. Check higlited elements");
               _waring = true;
            }
         }





         double startX = Math.Round(line2d.StartPoint.X, 3);
         double startY = Math.Round(line2d.StartPoint.Y, 3);
         updateMinMax(startX, startY);

         double endX = Math.Round(line2d.EndPoint.X, 3);
         double endY = Math.Round(line2d.EndPoint.Y, 3);
         updateMinMax(endX, endY);

         if (_first)
         {
            _outGCode.Add($"G0X{startX}Y{startY}");
            _outGCode.Add($"S{_power}");
            _outGCode.Add($"G1X{endX}Y{endY}F{_speed}");
            _first = false;
         }
         else
         {
            _outGCode.Add($"G1X{endX}Y{endY}");
         }
      }

      public void addArc(Polyline polyline, int index)
      {
         
        CircularArc3d arc3d = polyline.GetArcSegmentAt(index);

         double startX = Math.Round(arc3d.StartPoint.X, 3);
         double startY = Math.Round(arc3d.StartPoint.Y, 3);
         updateMinMax(startX, startY);

         double endX = Math.Round(arc3d.EndPoint.X, 3);
         double endY = Math.Round(arc3d.EndPoint.Y, 3);
         updateMinMax(endX, endY);

         double idX = Math.Round(arc3d.Center.X - startX, 3);
         double jdY = Math.Round(arc3d.Center.Y - startY, 3);
         string direction;



         if (arc3d.Normal.Z<0) direction = "G2";
         else direction = "G3";

         if (_first)
         {
            _outGCode.Add($"G0X{startX}Y{startY}");
            _outGCode.Add($"S{_power}");
            _outGCode.Add($"{direction}X{endX}Y{endY}I{idX}J{jdY}F{_speed}");
            _first = false;
         }
         else
         {
            _outGCode.Add($"{direction}X{endX}Y{endY}I{idX}J{jdY}");
         }
      }

      public void addCircle(Circle circle)
      {
         double startX = Math.Round(circle.StartPoint.X, 3);
         double startY = Math.Round(circle.StartPoint.Y, 3);
         
         double idX = Math.Round(circle.Center.X - startX, 3);
         double jdY = Math.Round(circle.Center.Y - startY, 3);

         double endX = Math.Round(circle.Center.X + idX, 3);
         double endY = Math.Round(circle.Center.Y + jdY, 3);

         double idXend = Math.Round(circle.Center.X - endX, 3);
         double jdYend = Math.Round(circle.Center.Y - endY, 3);

         updateMinMax(circle.Center.X+circle.Radius, startY+circle.Radius);
         updateMinMax(circle.Center.X-circle.Radius, startY-circle.Radius);

         _outGCode.Add($"G0X{startX}Y{startY}");
         _outGCode.Add($"S{_power}");
         _outGCode.Add($"G2X{endX}Y{endY}I{idX}J{jdY}F{_speed}");
         _outGCode.Add($"G2X{startX}Y{startY}I{idXend}J{jdYend}");

         //_outGCode.Add($"G2I{idX}J{jdY}F{_speed}");
      }

      private void updateMinMax(double x, double y)
      {
         if(x>maxX) maxX = Math.Round(x,3);
         if(y>maxY) maxY = Math.Round(y,3);
         if(x<minX) minX = Math.Round(x,3);
         if(y<minY) minY = Math.Round(y,3);

      }


   }
}