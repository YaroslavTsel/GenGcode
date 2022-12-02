using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Autodesk.AutoCAD.DatabaseServices;

namespace GenGcode
{
   public partial class SettingsForm : Form
   {
      private Database _db;

      public SettingsForm()
      {
         InitializeComponent();

         _db = HostApplicationServices.WorkingDatabase;

         Dictionary<string, string> props = Commands.ReadCustomProp(_db);

         var layersArray = LayersToList(_db).ToArray();

         comboBox1.Items.AddRange(layersArray);
         comboBox2.Items.AddRange(layersArray);
         comboBox3.Items.AddRange(layersArray);
         comboBox4.Items.AddRange(layersArray);

         int comboCount = 1;

         foreach (var layerName in layersArray)
         {
            int speed, power, repeat;

            if (Commands.ReadLayerParams(out speed, out power, out repeat, props, layerName))
            {
               comboCount = fillNextCombo(comboCount, layerName, speed, power, repeat);
            }

         }
      }

      private int fillNextCombo(int comboCount, string layerName, int speed, int power, int repeat)
      {
         switch (comboCount)
         {
            case 1:
               comboBox1.Text = layerName;
               SpeedUD1.Value = speed;
               PowerUD1.Value = power;
               RepeatUD1.Value = repeat;
               comboCount++;
               break;

            case 2:
               comboBox2.Text = layerName;
               SpeedUD2.Value = speed;
               PowerUD2.Value = power;
               RepeatUD2.Value = repeat;
               comboCount++;
               break;

            case 3:
               comboBox3.Text = layerName;
               SpeedUD3.Value = speed;
               PowerUD3.Value = power;
               RepeatUD3.Value = repeat;
               comboCount++;
               break;

            case 4:
               comboBox4.Text = layerName;
               SpeedUD4.Value = speed;
               PowerUD4.Value = power;
               RepeatUD4.Value = repeat;
               comboCount++;
               break;

            default:
               break;
         }

         return comboCount;
      }

      

      private void button1_Click(object sender, EventArgs e)
      {
         MessageBox.Show($"This program was written by Yaroslav Tselikovskiy\r\n Autocad and Revit C# developer\r\n bkramber@gmail.com");
      }

      private void button2_Click(object sender, EventArgs e)
      {
         var records = _db.SummaryInfo.CustomProperties;

         string cutProp = $"{SpeedUD1.Value};{PowerUD1.Value};{RepeatUD1.Value}";
         string engraveProp1 = $"{SpeedUD2.Value};{PowerUD2.Value};{RepeatUD2.Value}";
         string engraveProp2 = $"{SpeedUD3.Value};{PowerUD3.Value};{RepeatUD3.Value}";
         string engraveProp3 = $"{SpeedUD4.Value};{PowerUD4.Value};{RepeatUD4.Value}";

         if (comboBox1.Text != "") SetCustomProperty(_db, comboBox1.Text, cutProp);
         if (comboBox2.Text != "") SetCustomProperty(_db, comboBox2.Text, engraveProp1);
         if (comboBox3.Text != "") SetCustomProperty(_db, comboBox3.Text, engraveProp2);
         if (comboBox4.Text != "") SetCustomProperty(_db, comboBox4.Text, engraveProp3);
         this.Close();
      }

      public List<string> LayersToList(Database db)
      {
         List<string> lstlay = new List<string>();

         LayerTableRecord layer;
         using (Transaction tr = db.TransactionManager.StartOpenCloseTransaction())
         {
            LayerTable lt = tr.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;
            foreach (ObjectId layerId in lt)
            {
               layer = tr.GetObject(layerId, OpenMode.ForWrite) as LayerTableRecord;
               lstlay.Add(layer.Name);
            }
         }
         return lstlay;
      }

      public static void SetCustomProperty(Database db, string key, string value)
      {
         DatabaseSummaryInfoBuilder infoBuilder = new DatabaseSummaryInfoBuilder(db.SummaryInfo);
         var custProps = infoBuilder.CustomPropertyTable;
         if (custProps.Contains(key))
            custProps[key] = value;
         else
            custProps.Add(key, value);
         db.SummaryInfo = infoBuilder.ToDatabaseSummaryInfo();
      }

   }
}