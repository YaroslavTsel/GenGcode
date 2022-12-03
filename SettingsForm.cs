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

         LoadFromCustomProps(Commands.ReadCustomProp(_db));

      }

      private void LoadFromCustomProps(SortedDictionary<string, string> props)
      {

         var layersArray = LayersToList(_db).ToArray();

         comboBox1.Items.AddRange(layersArray);
         comboBox2.Items.AddRange(layersArray);
         comboBox3.Items.AddRange(layersArray);
         comboBox4.Items.AddRange(layersArray);

         comboBox5.Items.AddRange(layersArray);
         comboBox6.Items.AddRange(layersArray);
         comboBox7.Items.AddRange(layersArray);
         comboBox8.Items.AddRange(layersArray);

         int comboCount = 1;

         foreach (var layerName in layersArray)
         {
            int speed, power, repeat;

            if (Commands.ReadLayerParams(out speed, out power, out repeat, props, layerName))
            {
               comboCount = fillNextCombo( layerName, speed, power, repeat);
            }

         }

         int fillNextCombo(string layerName, int speed, int power, int repeat)
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

               case 5:
                  comboBox5.Text = layerName;
                  SpeedUD5.Value = speed;
                  PowerUD5.Value = power;
                  RepeatUD5.Value = repeat;
                  comboCount++;
                  break;

               case 6:
                  comboBox6.Text = layerName;
                  SpeedUD6.Value = speed;
                  PowerUD6.Value = power;
                  RepeatUD6.Value = repeat;
                  comboCount++;
                  break;


               case 7:
                  comboBox7.Text = layerName;
                  SpeedUD7.Value = speed;
                  PowerUD7.Value = power;
                  RepeatUD7.Value = repeat;
                  comboCount++;
                  break;


               case 8:
                  comboBox8.Text = layerName;
                  SpeedUD8.Value = speed;
                  PowerUD8.Value = power;
                  RepeatUD8.Value = repeat;
                  comboCount++;
                  break;

               default:
                  break;
            }

            return comboCount;
         }

      }





      private void InfoBtn_Click(object sender, EventArgs e)
      {
         MessageBox.Show($"This program was written by Yaroslav Tselikovskiy\r\n Autocad and Revit C# developer\r\n bkramber@gmail.com");
      }

      private void saveProp_Click(object sender, EventArgs e)
      {
         DeleteCustomPropertys(_db);

         string engraveProp1, engraveProp2, engraveProp3, engraveProp4, engraveProp5, engraveProp6, engraveProp7, engraveProp8;

         CombinePropStrings();

         SaveToCustomProp();

         this.Close();

         void CombinePropStrings()
         {
            engraveProp1 = $"{SpeedUD1.Value};{PowerUD1.Value};{RepeatUD1.Value}";
            engraveProp2 = $"{SpeedUD2.Value};{PowerUD2.Value};{RepeatUD2.Value}";
            engraveProp3 = $"{SpeedUD3.Value};{PowerUD3.Value};{RepeatUD3.Value}";
            engraveProp4 = $"{SpeedUD4.Value};{PowerUD4.Value};{RepeatUD4.Value}";
            engraveProp5 = $"{SpeedUD5.Value};{PowerUD5.Value};{RepeatUD5.Value}";
            engraveProp6 = $"{SpeedUD6.Value};{PowerUD6.Value};{RepeatUD6.Value}";
            engraveProp7 = $"{SpeedUD7.Value};{PowerUD7.Value};{RepeatUD7.Value}";
            engraveProp8 = $"{SpeedUD8.Value};{PowerUD8.Value};{RepeatUD8.Value}";
         }

         void SaveToCustomProp()
         {
            if (comboBox1.Text != "") SetCustomProperty(_db, comboBox1.Text, engraveProp1);
            if (comboBox2.Text != "") SetCustomProperty(_db, comboBox2.Text, engraveProp2);
            if (comboBox3.Text != "") SetCustomProperty(_db, comboBox3.Text, engraveProp3);
            if (comboBox4.Text != "") SetCustomProperty(_db, comboBox4.Text, engraveProp4);

            if (comboBox5.Text != "") SetCustomProperty(_db, comboBox5.Text, engraveProp5);
            if (comboBox6.Text != "") SetCustomProperty(_db, comboBox6.Text, engraveProp6);
            if (comboBox7.Text != "") SetCustomProperty(_db, comboBox7.Text, engraveProp7);
            if (comboBox8.Text != "") SetCustomProperty(_db, comboBox8.Text, engraveProp8);
         }
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
      public static void DeleteCustomPropertys(Database db)
      {
         DatabaseSummaryInfoBuilder infoBuilder = new DatabaseSummaryInfoBuilder(db.SummaryInfo);
         var custProps = infoBuilder.CustomPropertyTable;
         custProps.Clear();
         db.SummaryInfo = infoBuilder.ToDatabaseSummaryInfo();
      }

   }
}