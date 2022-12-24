using System;
using System.Collections.Generic;

using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace GenGcode
{
   internal class Gcode
   {
      private List<string> _outGCode = new List<string>();
      private bool _first = true;
      private int _speed, _power;

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

      public void addLine(LineSegment2d line)
      {
         double startX = Math.Round(line.StartPoint.X, 3);
         double startY = Math.Round(line.StartPoint.Y, 3);
         updateMinMax(startX, startY);

         double endX = Math.Round(line.EndPoint.X, 3);
         double endY = Math.Round(line.EndPoint.Y, 3);
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

      public void addArc(CircularArc2d arc)
      {
         double startX = Math.Round(arc.StartPoint.X, 3);
         double startY = Math.Round(arc.StartPoint.Y, 3);
         updateMinMax(startX, startY);

         double endX = Math.Round(arc.EndPoint.X, 3);
         double endY = Math.Round(arc.EndPoint.Y, 3);
         updateMinMax(endX, endY);

         double idX = Math.Round(arc.Center.X - startX, 3);
         double jdY = Math.Round(arc.Center.Y - startY, 3);

         string direction;

         if (arc.IsClockWise) direction = "G2";
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

         updateMinMax(circle.Center.X+circle.Radius, startY+circle.Radius);
         updateMinMax(circle.Center.X-circle.Radius, startY-circle.Radius);

         _outGCode.Add($"G0X{startX}Y{startY}");
         _outGCode.Add($"S{_power}");
         _outGCode.Add($"G2I{idX}J{jdY}F{_speed}");
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