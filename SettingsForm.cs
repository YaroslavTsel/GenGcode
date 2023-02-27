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
         PassByBox.Checked = GetGcode._passByEntity;

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


         foreach (var prop in props)
         {
            comboCount = fillNextCombo(prop);
         }

         int fillNextCombo(KeyValuePair<string, string> prop)
         {
            string layerName;
            int speed, power, repeat;
            bool dynPwr;

            DecodeProp(prop, out layerName, out speed, out power, out repeat,out dynPwr);

            switch (comboCount)
            {
               case 1:
                  comboBox1.Text = layerName;
                  SpeedUD1.Value = speed;
                  PowerUD1.Value = power;
                  RepeatUD1.Value = repeat;
                  DynamPwrBox1.Checked = dynPwr;
                  comboCount++;
                  break;

               case 2:
                  comboBox2.Text = layerName;
                  SpeedUD2.Value = speed;
                  PowerUD2.Value = power;
                  RepeatUD2.Value = repeat;
                  DynamPwrBox2.Checked = dynPwr;
                  comboCount++;
                  break;

               case 3:
                  comboBox3.Text = layerName;
                  SpeedUD3.Value = speed;
                  PowerUD3.Value = power;
                  RepeatUD3.Value = repeat;
                  DynamPwrBox3.Checked = dynPwr;
                  comboCount++;
                  break;

               case 4:
                  comboBox4.Text = layerName;
                  SpeedUD4.Value = speed;
                  PowerUD4.Value = power;
                  RepeatUD4.Value = repeat;
                  DynamPwrBox4.Checked = dynPwr;
                  comboCount++;
                  break;

               case 5:
                  comboBox5.Text = layerName;
                  SpeedUD5.Value = speed;
                  PowerUD5.Value = power;
                  RepeatUD5.Value = repeat;
                  DynamPwrBox5.Checked = dynPwr;
                  comboCount++;
                  break;

               case 6:
                  comboBox6.Text = layerName;
                  SpeedUD6.Value = speed;
                  PowerUD6.Value = power;
                  RepeatUD6.Value = repeat;
                  DynamPwrBox6.Checked = dynPwr;
                  comboCount++;
                  break;


               case 7:
                  comboBox7.Text = layerName;
                  SpeedUD7.Value = speed;
                  PowerUD7.Value = power;
                  RepeatUD7.Value = repeat;
                  DynamPwrBox7.Checked = dynPwr;
                  comboCount++;
                  break;


               case 8:
                  comboBox8.Text = layerName;
                  SpeedUD8.Value = speed;
                  PowerUD8.Value = power;
                  RepeatUD8.Value = repeat;
                  DynamPwrBox8.Checked = dynPwr;
                  comboCount++;
                  break;

               default:
                  break;
            }

            return comboCount;
         }

      }

      public static void DecodeProp(KeyValuePair<string, string> prop, out string layerName, out int speed, out int power, out int repeat, out bool dynPwr)
      {
         var values = prop.Value.Split(';');

         layerName = values[0];
         speed = Convert.ToInt32(values[1]);
         power = Convert.ToInt32(values[2]);
         repeat = Convert.ToInt32(values[3]);
         dynPwr = Convert.ToBoolean(values[4]);
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
            engraveProp1 = $"{comboBox1.Text};{SpeedUD1.Value};{PowerUD1.Value};{RepeatUD1.Value};{DynamPwrBox1.Checked}";
            engraveProp2 = $"{comboBox2.Text};{SpeedUD2.Value};{PowerUD2.Value};{RepeatUD2.Value};{DynamPwrBox2.Checked}";
            engraveProp3 = $"{comboBox3.Text};{SpeedUD3.Value};{PowerUD3.Value};{RepeatUD3.Value};{DynamPwrBox3.Checked}";
            engraveProp4 = $"{comboBox4.Text};{SpeedUD4.Value};{PowerUD4.Value};{RepeatUD4.Value};{DynamPwrBox4.Checked}";
            engraveProp5 = $"{comboBox5.Text};{SpeedUD5.Value};{PowerUD5.Value};{RepeatUD5.Value};{DynamPwrBox5.Checked}";
            engraveProp6 = $"{comboBox6.Text};{SpeedUD6.Value};{PowerUD6.Value};{RepeatUD6.Value};{DynamPwrBox6.Checked}";
            engraveProp7 = $"{comboBox7.Text};{SpeedUD7.Value};{PowerUD7.Value};{RepeatUD7.Value};{DynamPwrBox7.Checked}";
            engraveProp8 = $"{comboBox8.Text};{SpeedUD8.Value};{PowerUD8.Value};{RepeatUD8.Value};{DynamPwrBox8.Checked}";
         }

         void SaveToCustomProp()
         {
            if (comboBox1.Text != "") SetCustomProperty(_db, "EngraveProp1", engraveProp1);
            if (comboBox2.Text != "") SetCustomProperty(_db, "EngraveProp2", engraveProp2);
            if (comboBox3.Text != "") SetCustomProperty(_db, "EngraveProp3", engraveProp3);
            if (comboBox4.Text != "") SetCustomProperty(_db, "EngraveProp4", engraveProp4);

            if (comboBox5.Text != "") SetCustomProperty(_db, "EngraveProp5", engraveProp5);
            if (comboBox6.Text != "") SetCustomProperty(_db, "EngraveProp6", engraveProp6);
            if (comboBox7.Text != "") SetCustomProperty(_db, "EngraveProp7", engraveProp7);
            if (comboBox8.Text != "") SetCustomProperty(_db, "EngraveProp8", engraveProp8);
            SetCustomProperty(_db, "PassByElement", PassByBox.Checked.ToString());
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

      private void label6_Click(object sender, EventArgs e)
      {

      }

      private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
      {
         if (RepeatUD2.Value == 0) RepeatUD2.Value = 1;
      }
      private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
      {
         if (RepeatUD3.Value == 0) RepeatUD3.Value = 1;
      }

      private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
      {
         if (RepeatUD4.Value == 0) RepeatUD4.Value = 1;
      }

      private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
      {
         if (RepeatUD5.Value == 0) RepeatUD5.Value = 1;
      }

      private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
      {
         if (RepeatUD6.Value == 0) RepeatUD6.Value = 1;
      }

      private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
      {
         if (RepeatUD7.Value == 0) RepeatUD7.Value = 1;
      }

      private void comboBox8_SelectedIndexChanged(object sender, EventArgs e)
      {
         if (RepeatUD8.Value == 0) RepeatUD8.Value = 1;
      }
   
   }
}