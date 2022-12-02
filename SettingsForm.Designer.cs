namespace GenGcode
{
   partial class SettingsForm
   {
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
      protected override void Dispose(bool disposing)
      {
         if (disposing && (components != null))
         {
            components.Dispose();
         }
         base.Dispose(disposing);
      }

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.comboBox1 = new System.Windows.Forms.ComboBox();
         this.label1 = new System.Windows.Forms.Label();
         this.label2 = new System.Windows.Forms.Label();
         this.label3 = new System.Windows.Forms.Label();
         this.label4 = new System.Windows.Forms.Label();
         this.RepeatUD1 = new System.Windows.Forms.NumericUpDown();
         this.PowerUD1 = new System.Windows.Forms.NumericUpDown();
         this.SpeedUD1 = new System.Windows.Forms.NumericUpDown();
         this.SpeedUD2 = new System.Windows.Forms.NumericUpDown();
         this.PowerUD2 = new System.Windows.Forms.NumericUpDown();
         this.RepeatUD2 = new System.Windows.Forms.NumericUpDown();
         this.comboBox2 = new System.Windows.Forms.ComboBox();
         this.SpeedUD3 = new System.Windows.Forms.NumericUpDown();
         this.PowerUD3 = new System.Windows.Forms.NumericUpDown();
         this.RepeatUD3 = new System.Windows.Forms.NumericUpDown();
         this.comboBox3 = new System.Windows.Forms.ComboBox();
         this.comboBox4 = new System.Windows.Forms.ComboBox();
         this.RepeatUD4 = new System.Windows.Forms.NumericUpDown();
         this.PowerUD4 = new System.Windows.Forms.NumericUpDown();
         this.SpeedUD4 = new System.Windows.Forms.NumericUpDown();
         this.button1 = new System.Windows.Forms.Button();
         this.button2 = new System.Windows.Forms.Button();
         ((System.ComponentModel.ISupportInitialize)(this.RepeatUD1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.PowerUD1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.SpeedUD1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.SpeedUD2)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.PowerUD2)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.RepeatUD2)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.SpeedUD3)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.PowerUD3)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.RepeatUD3)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.RepeatUD4)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.PowerUD4)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.SpeedUD4)).BeginInit();
         this.SuspendLayout();
         // 
         // comboBox1
         // 
         this.comboBox1.FormattingEnabled = true;
         this.comboBox1.Location = new System.Drawing.Point(12, 36);
         this.comboBox1.Name = "comboBox1";
         this.comboBox1.Size = new System.Drawing.Size(367, 28);
         this.comboBox1.TabIndex = 0;
         // 
         // label1
         // 
         this.label1.AutoSize = true;
         this.label1.Location = new System.Drawing.Point(8, 11);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(48, 20);
         this.label1.TabIndex = 1;
         this.label1.Text = "Layer";
         // 
         // label2
         // 
         this.label2.AutoSize = true;
         this.label2.Location = new System.Drawing.Point(381, 10);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(56, 20);
         this.label2.TabIndex = 1;
         this.label2.Text = "Speed";
         // 
         // label3
         // 
         this.label3.AutoSize = true;
         this.label3.Location = new System.Drawing.Point(486, 10);
         this.label3.Name = "label3";
         this.label3.Size = new System.Drawing.Size(71, 20);
         this.label3.TabIndex = 1;
         this.label3.Text = "Power,%";
         // 
         // label4
         // 
         this.label4.AutoSize = true;
         this.label4.Location = new System.Drawing.Point(575, 10);
         this.label4.Name = "label4";
         this.label4.Size = new System.Drawing.Size(62, 20);
         this.label4.TabIndex = 1;
         this.label4.Text = "Repeat";
         // 
         // RepeatUD1
         // 
         this.RepeatUD1.Location = new System.Drawing.Point(579, 35);
         this.RepeatUD1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
         this.RepeatUD1.Name = "RepeatUD1";
         this.RepeatUD1.Size = new System.Drawing.Size(83, 26);
         this.RepeatUD1.TabIndex = 3;
         this.RepeatUD1.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
         // 
         // PowerUD1
         // 
         this.PowerUD1.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
         this.PowerUD1.Location = new System.Drawing.Point(490, 36);
         this.PowerUD1.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
         this.PowerUD1.Name = "PowerUD1";
         this.PowerUD1.Size = new System.Drawing.Size(83, 26);
         this.PowerUD1.TabIndex = 3;
         this.PowerUD1.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
         // 
         // SpeedUD1
         // 
         this.SpeedUD1.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
         this.SpeedUD1.Location = new System.Drawing.Point(385, 36);
         this.SpeedUD1.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
         this.SpeedUD1.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
         this.SpeedUD1.Name = "SpeedUD1";
         this.SpeedUD1.Size = new System.Drawing.Size(99, 26);
         this.SpeedUD1.TabIndex = 3;
         this.SpeedUD1.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
         // 
         // SpeedUD2
         // 
         this.SpeedUD2.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
         this.SpeedUD2.Location = new System.Drawing.Point(385, 70);
         this.SpeedUD2.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
         this.SpeedUD2.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
         this.SpeedUD2.Name = "SpeedUD2";
         this.SpeedUD2.Size = new System.Drawing.Size(99, 26);
         this.SpeedUD2.TabIndex = 6;
         this.SpeedUD2.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
         // 
         // PowerUD2
         // 
         this.PowerUD2.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
         this.PowerUD2.Location = new System.Drawing.Point(490, 70);
         this.PowerUD2.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
         this.PowerUD2.Name = "PowerUD2";
         this.PowerUD2.Size = new System.Drawing.Size(83, 26);
         this.PowerUD2.TabIndex = 7;
         this.PowerUD2.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
         // 
         // RepeatUD2
         // 
         this.RepeatUD2.Location = new System.Drawing.Point(579, 69);
         this.RepeatUD2.Name = "RepeatUD2";
         this.RepeatUD2.Size = new System.Drawing.Size(83, 26);
         this.RepeatUD2.TabIndex = 8;
         // 
         // comboBox2
         // 
         this.comboBox2.FormattingEnabled = true;
         this.comboBox2.Location = new System.Drawing.Point(12, 70);
         this.comboBox2.Name = "comboBox2";
         this.comboBox2.Size = new System.Drawing.Size(367, 28);
         this.comboBox2.TabIndex = 5;
         // 
         // SpeedUD3
         // 
         this.SpeedUD3.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
         this.SpeedUD3.Location = new System.Drawing.Point(385, 104);
         this.SpeedUD3.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
         this.SpeedUD3.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
         this.SpeedUD3.Name = "SpeedUD3";
         this.SpeedUD3.Size = new System.Drawing.Size(99, 26);
         this.SpeedUD3.TabIndex = 11;
         this.SpeedUD3.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
         // 
         // PowerUD3
         // 
         this.PowerUD3.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
         this.PowerUD3.Location = new System.Drawing.Point(490, 104);
         this.PowerUD3.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
         this.PowerUD3.Name = "PowerUD3";
         this.PowerUD3.Size = new System.Drawing.Size(83, 26);
         this.PowerUD3.TabIndex = 12;
         this.PowerUD3.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
         // 
         // RepeatUD3
         // 
         this.RepeatUD3.Location = new System.Drawing.Point(579, 103);
         this.RepeatUD3.Name = "RepeatUD3";
         this.RepeatUD3.Size = new System.Drawing.Size(83, 26);
         this.RepeatUD3.TabIndex = 13;
         // 
         // comboBox3
         // 
         this.comboBox3.FormattingEnabled = true;
         this.comboBox3.Location = new System.Drawing.Point(12, 104);
         this.comboBox3.Name = "comboBox3";
         this.comboBox3.Size = new System.Drawing.Size(367, 28);
         this.comboBox3.TabIndex = 10;
         // 
         // comboBox4
         // 
         this.comboBox4.FormattingEnabled = true;
         this.comboBox4.Location = new System.Drawing.Point(12, 138);
         this.comboBox4.Name = "comboBox4";
         this.comboBox4.Size = new System.Drawing.Size(367, 28);
         this.comboBox4.TabIndex = 10;
         // 
         // RepeatUD4
         // 
         this.RepeatUD4.Location = new System.Drawing.Point(579, 137);
         this.RepeatUD4.Name = "RepeatUD4";
         this.RepeatUD4.Size = new System.Drawing.Size(83, 26);
         this.RepeatUD4.TabIndex = 13;
         // 
         // PowerUD4
         // 
         this.PowerUD4.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
         this.PowerUD4.Location = new System.Drawing.Point(490, 138);
         this.PowerUD4.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
         this.PowerUD4.Name = "PowerUD4";
         this.PowerUD4.Size = new System.Drawing.Size(83, 26);
         this.PowerUD4.TabIndex = 12;
         this.PowerUD4.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
         // 
         // SpeedUD4
         // 
         this.SpeedUD4.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
         this.SpeedUD4.Location = new System.Drawing.Point(385, 138);
         this.SpeedUD4.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
         this.SpeedUD4.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
         this.SpeedUD4.Name = "SpeedUD4";
         this.SpeedUD4.Size = new System.Drawing.Size(99, 26);
         this.SpeedUD4.TabIndex = 11;
         this.SpeedUD4.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
         // 
         // button1
         // 
         this.button1.Location = new System.Drawing.Point(12, 206);
         this.button1.Name = "button1";
         this.button1.Size = new System.Drawing.Size(117, 40);
         this.button1.TabIndex = 15;
         this.button1.Text = "About me";
         this.button1.UseVisualStyleBackColor = true;
         this.button1.Click += new System.EventHandler(this.button1_Click);
         // 
         // button2
         // 
         this.button2.Location = new System.Drawing.Point(545, 206);
         this.button2.Name = "button2";
         this.button2.Size = new System.Drawing.Size(117, 40);
         this.button2.TabIndex = 15;
         this.button2.Text = "Save";
         this.button2.UseVisualStyleBackColor = true;
         this.button2.Click += new System.EventHandler(this.button2_Click);
         // 
         // SettingsForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(694, 298);
         this.Controls.Add(this.button2);
         this.Controls.Add(this.button1);
         this.Controls.Add(this.SpeedUD4);
         this.Controls.Add(this.SpeedUD3);
         this.Controls.Add(this.PowerUD4);
         this.Controls.Add(this.PowerUD3);
         this.Controls.Add(this.RepeatUD4);
         this.Controls.Add(this.RepeatUD3);
         this.Controls.Add(this.comboBox4);
         this.Controls.Add(this.comboBox3);
         this.Controls.Add(this.SpeedUD2);
         this.Controls.Add(this.PowerUD2);
         this.Controls.Add(this.RepeatUD2);
         this.Controls.Add(this.comboBox2);
         this.Controls.Add(this.SpeedUD1);
         this.Controls.Add(this.PowerUD1);
         this.Controls.Add(this.RepeatUD1);
         this.Controls.Add(this.label4);
         this.Controls.Add(this.label3);
         this.Controls.Add(this.label2);
         this.Controls.Add(this.label1);
         this.Controls.Add(this.comboBox1);
         this.Name = "SettingsForm";
         this.Text = "Settings";
         ((System.ComponentModel.ISupportInitialize)(this.RepeatUD1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.PowerUD1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.SpeedUD1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.SpeedUD2)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.PowerUD2)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.RepeatUD2)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.SpeedUD3)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.PowerUD3)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.RepeatUD3)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.RepeatUD4)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.PowerUD4)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.SpeedUD4)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.ComboBox comboBox1;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.Label label4;
      private System.Windows.Forms.NumericUpDown RepeatUD1;
      private System.Windows.Forms.NumericUpDown PowerUD1;
      private System.Windows.Forms.NumericUpDown SpeedUD1;
      private System.Windows.Forms.NumericUpDown SpeedUD2;
      private System.Windows.Forms.NumericUpDown PowerUD2;
      private System.Windows.Forms.NumericUpDown RepeatUD2;
      private System.Windows.Forms.ComboBox comboBox2;
      private System.Windows.Forms.NumericUpDown SpeedUD3;
      private System.Windows.Forms.NumericUpDown PowerUD3;
      private System.Windows.Forms.NumericUpDown RepeatUD3;
      private System.Windows.Forms.ComboBox comboBox3;
      private System.Windows.Forms.ComboBox comboBox4;
      private System.Windows.Forms.NumericUpDown RepeatUD4;
      private System.Windows.Forms.NumericUpDown PowerUD4;
      private System.Windows.Forms.NumericUpDown SpeedUD4;
      private System.Windows.Forms.Button button1;
      private System.Windows.Forms.Button button2;
   }
}