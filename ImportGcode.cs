using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace GenGcode
{
   public class ImportGcode
   {
      // open file

      public ImportGcode()
      {
         List<(Point3d, Point3d)> coordList = LoadGcode();

         // Get the current document and database
         Document acDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
         Database acCurDb = acDoc.Database;

         // Start a transaction
         using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
         {
            // Open the Block table for read
            BlockTable acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                                         OpenMode.ForRead) as BlockTable;

            // Open the Block table record Model space for write
            BlockTableRecord acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                            OpenMode.ForWrite) as BlockTableRecord;

            foreach (var coord in coordList)
            {
               DrawLine(coord.Item1, coord.Item2, acBlkTblRec, acTrans);
            }

            acTrans.Commit();
         }
      }

      private void DrawLine(Point3d startPoint, Point3d endPoint, BlockTableRecord acBlkTblRec, Transaction acTrans)
      {
         if (startPoint != endPoint)
         {
            // Create a new line that starts
            Line acLine = new Line(startPoint,
                                   endPoint);
            // Add the new line to the block table record and the transaction
            acBlkTblRec.AppendEntity(acLine);
            acTrans.AddNewlyCreatedDBObject(acLine, true);
         }
      }

      private List<(Point3d, Point3d)> LoadGcode()
      {
         List<(Point3d, Point3d)> result = new List<(Point3d, Point3d)>();

         OpenFileDialog openFileDialog = new OpenFileDialog();
         openFileDialog.Filter = "GCODE files(*.nc)|*.nc|All files(*.*)|*.*";

         if (openFileDialog.ShowDialog() == DialogResult.Cancel)
            return null;

         string filename = openFileDialog.FileName;

         int totalStrCount = System.IO.File.ReadAllLines(@filename).Length;

         FileStream fs = new FileStream(@filename, FileMode.Open);
         StreamReader streamReader = new StreamReader(fs);

         (Point3d, Point3d) linesCoords = default;

         Point3d startPoint = default;

         List<string> coordLines = new List<string>();

         for (int strCounter = 0; strCounter < totalStrCount; strCounter++)
         {
            string newLine = streamReader.ReadLine();

            if (newLine.StartsWith("G1"))
            {
               coordLines.Add(newLine);
            }

            if (newLine.StartsWith("G0"))
            {
               if (coordLines.Count > 0)
               {
                  linesCoords.Item1 = startPoint;
                  linesCoords.Item2 = GetPoint3D(coordLines[0]);
                  result.Add(linesCoords);
               }

               for (int i = 1; i < coordLines.Count; i++)
               {
                  linesCoords.Item1 = GetPoint3D(coordLines[i - 1]);
                  linesCoords.Item2 = GetPoint3D(coordLines[i]);
                  result.Add(linesCoords);
               }
               coordLines.Clear();

               startPoint = GetPoint3D(newLine);

            }
         }

         fs.Close();

         return result;
      }

      private static Point3d GetPoint3D(string inputStr)
      {
         string subStr = inputStr.Split('X')[1];
         string[] subXYstr = subStr.Split('Y');

         return new Point3d(Convert.ToDouble(subXYstr[0]), Convert.ToDouble(subXYstr[1].Split('F')[0]), 0);
      }
   }
}