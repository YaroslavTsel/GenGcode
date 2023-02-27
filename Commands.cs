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
   public class Commands : IExtensionApplication
   {
      
      

      public void Initialize()
      {
         MessageBox.Show("GenGcode loaded. \r\n Type \"GETGCODE\" to run \r\n, \"SETGCODE\" to setup or  \r\n\"IMPORTGCODE\" for import *.nc file");
      }


      [CommandMethod("GETGCODE", CommandFlags.UsePickSet)]
      public void GetGcodeCmd()
      {
         GetGcode getGcode = new GetGcode();
      }
            

      [CommandMethod("SETGCODE")]
      public void SetGcodeCmd()
      {
         Database db = HostApplicationServices.WorkingDatabase;
         using (GenGcode.SettingsForm setGcodeForm = new GenGcode.SettingsForm())
         {
            setGcodeForm.ShowDialog();
         }
      }


      [CommandMethod("IMPORTGCODE", CommandFlags.UsePickSet)]
      public void ImportGcodeCmd()
      {
         ImportGcode import = new ImportGcode();
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
               GetGcode._passByEntity = Convert.ToBoolean(records.Value);
            }
         }

         return customProp;
      }

         public void Terminate()
      {
      }
   }
}