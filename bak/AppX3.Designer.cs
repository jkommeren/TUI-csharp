namespace TangibleUISharp
{
    partial class AppX3
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
            this.trackSizeMax = new System.Windows.Forms.TrackBar();
            this.trackSizeMin = new System.Windows.Forms.TrackBar();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.trackValMax = new System.Windows.Forms.TrackBar();
            this.trackSatMax = new System.Windows.Forms.TrackBar();
            this.trackHueMax = new System.Windows.Forms.TrackBar();
            this.trackValMin = new System.Windows.Forms.TrackBar();
            this.trackSatMin = new System.Windows.Forms.TrackBar();
            this.trackHueMin = new System.Windows.Forms.TrackBar();
            this.hueMin = new System.Windows.Forms.Label();
            this.satMin = new System.Windows.Forms.Label();
            this.sizeMin = new System.Windows.Forms.Label();
            this.valMin = new System.Windows.Forms.Label();
            this.sizeMax = new System.Windows.Forms.Label();
            this.valMax = new System.Windows.Forms.Label();
            this.satMax = new System.Windows.Forms.Label();
            this.hueMax = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.trackSizeMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackSizeMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackValMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackSatMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackHueMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackValMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackSatMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackHueMin)).BeginInit();
            this.SuspendLayout();
            // 
            // trackSizeMax
            // 
            this.trackSizeMax.LargeChange = 5000;
            this.trackSizeMax.Location = new System.Drawing.Point(269, 167);
            this.trackSizeMax.Maximum = 50000;
            this.trackSizeMax.Name = "trackSizeMax";
            this.trackSizeMax.Size = new System.Drawing.Size(104, 45);
            this.trackSizeMax.TabIndex = 23;
            this.trackSizeMax.Value = 20000;
            this.trackSizeMax.Scroll += new System.EventHandler(this.trackSizeMax_Scroll);
            // 
            // trackSizeMin
            // 
            this.trackSizeMin.LargeChange = 500;
            this.trackSizeMin.Location = new System.Drawing.Point(91, 167);
            this.trackSizeMin.Maximum = 2000;
            this.trackSizeMin.Name = "trackSizeMin";
            this.trackSizeMin.Size = new System.Drawing.Size(104, 45);
            this.trackSizeMin.TabIndex = 22;
            this.trackSizeMin.Value = 2000;
            this.trackSizeMin.Scroll += new System.EventHandler(this.trackSizeMin_Scroll);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(195, 167);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 13);
            this.label4.TabIndex = 21;
            this.label4.Text = "Obj Size";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(195, 130);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 20;
            this.label3.Text = "Value";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(195, 94);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 19;
            this.label2.Text = "Saturation";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(195, 62);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 13);
            this.label1.TabIndex = 18;
            this.label1.Text = "Hue";
            // 
            // trackValMax
            // 
            this.trackValMax.LargeChange = 50;
            this.trackValMax.Location = new System.Drawing.Point(269, 130);
            this.trackValMax.Maximum = 255;
            this.trackValMax.Name = "trackValMax";
            this.trackValMax.Size = new System.Drawing.Size(104, 45);
            this.trackValMax.TabIndex = 17;
            this.trackValMax.Value = 255;
            this.trackValMax.Scroll += new System.EventHandler(this.trackValMax_Scroll);
            // 
            // trackSatMax
            // 
            this.trackSatMax.LargeChange = 50;
            this.trackSatMax.Location = new System.Drawing.Point(269, 94);
            this.trackSatMax.Maximum = 255;
            this.trackSatMax.Name = "trackSatMax";
            this.trackSatMax.Size = new System.Drawing.Size(104, 45);
            this.trackSatMax.TabIndex = 16;
            this.trackSatMax.Value = 255;
            this.trackSatMax.Scroll += new System.EventHandler(this.trackSatMax_Scroll);
            // 
            // trackHueMax
            // 
            this.trackHueMax.LargeChange = 50;
            this.trackHueMax.Location = new System.Drawing.Point(269, 62);
            this.trackHueMax.Maximum = 255;
            this.trackHueMax.Name = "trackHueMax";
            this.trackHueMax.Size = new System.Drawing.Size(104, 45);
            this.trackHueMax.TabIndex = 15;
            this.trackHueMax.Value = 255;
            this.trackHueMax.Scroll += new System.EventHandler(this.trackHueMax_Scroll);
            // 
            // trackValMin
            // 
            this.trackValMin.LargeChange = 50;
            this.trackValMin.Location = new System.Drawing.Point(91, 130);
            this.trackValMin.Maximum = 255;
            this.trackValMin.Name = "trackValMin";
            this.trackValMin.Size = new System.Drawing.Size(104, 45);
            this.trackValMin.TabIndex = 14;
            this.trackValMin.Value = 1;
            this.trackValMin.Scroll += new System.EventHandler(this.trackValMin_Scroll);
            // 
            // trackSatMin
            // 
            this.trackSatMin.LargeChange = 50;
            this.trackSatMin.Location = new System.Drawing.Point(91, 97);
            this.trackSatMin.Maximum = 255;
            this.trackSatMin.Name = "trackSatMin";
            this.trackSatMin.Size = new System.Drawing.Size(104, 45);
            this.trackSatMin.TabIndex = 13;
            this.trackSatMin.Value = 1;
            this.trackSatMin.Scroll += new System.EventHandler(this.trackSatMin_Scroll);
            // 
            // trackHueMin
            // 
            this.trackHueMin.LargeChange = 50;
            this.trackHueMin.Location = new System.Drawing.Point(91, 62);
            this.trackHueMin.Maximum = 255;
            this.trackHueMin.Name = "trackHueMin";
            this.trackHueMin.Size = new System.Drawing.Size(104, 45);
            this.trackHueMin.TabIndex = 12;
            this.trackHueMin.Value = 1;
            this.trackHueMin.Scroll += new System.EventHandler(this.trackHueMin_Scroll);
            // 
            // hueMin
            // 
            this.hueMin.AutoSize = true;
            this.hueMin.Location = new System.Drawing.Point(40, 62);
            this.hueMin.Name = "hueMin";
            this.hueMin.Size = new System.Drawing.Size(45, 13);
            this.hueMin.TabIndex = 24;
            this.hueMin.Text = "<value>";
            // 
            // satMin
            // 
            this.satMin.AutoSize = true;
            this.satMin.Location = new System.Drawing.Point(40, 97);
            this.satMin.Name = "satMin";
            this.satMin.Size = new System.Drawing.Size(45, 13);
            this.satMin.TabIndex = 25;
            this.satMin.Text = "<value>";
            // 
            // sizeMin
            // 
            this.sizeMin.AutoSize = true;
            this.sizeMin.Location = new System.Drawing.Point(40, 167);
            this.sizeMin.Name = "sizeMin";
            this.sizeMin.Size = new System.Drawing.Size(45, 13);
            this.sizeMin.TabIndex = 29;
            this.sizeMin.Text = "<value>";
            // 
            // valMin
            // 
            this.valMin.AutoSize = true;
            this.valMin.Location = new System.Drawing.Point(40, 130);
            this.valMin.Name = "valMin";
            this.valMin.Size = new System.Drawing.Size(45, 13);
            this.valMin.TabIndex = 30;
            this.valMin.Text = "<value>";
            // 
            // sizeMax
            // 
            this.sizeMax.AutoSize = true;
            this.sizeMax.Location = new System.Drawing.Point(379, 162);
            this.sizeMax.Name = "sizeMax";
            this.sizeMax.Size = new System.Drawing.Size(45, 13);
            this.sizeMax.TabIndex = 31;
            this.sizeMax.Text = "<value>";
            // 
            // valMax
            // 
            this.valMax.AutoSize = true;
            this.valMax.Location = new System.Drawing.Point(379, 130);
            this.valMax.Name = "valMax";
            this.valMax.Size = new System.Drawing.Size(45, 13);
            this.valMax.TabIndex = 32;
            this.valMax.Text = "<value>";
            // 
            // satMax
            // 
            this.satMax.AutoSize = true;
            this.satMax.Location = new System.Drawing.Point(379, 97);
            this.satMax.Name = "satMax";
            this.satMax.Size = new System.Drawing.Size(45, 13);
            this.satMax.TabIndex = 33;
            this.satMax.Text = "<value>";
            // 
            // hueMax
            // 
            this.hueMax.AutoSize = true;
            this.hueMax.Location = new System.Drawing.Point(379, 62);
            this.hueMax.Name = "hueMax";
            this.hueMax.Size = new System.Drawing.Size(45, 13);
            this.hueMax.TabIndex = 34;
            this.hueMax.Text = "<value>";
            // 
            // AppX3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(482, 314);
            this.Controls.Add(this.hueMax);
            this.Controls.Add(this.satMax);
            this.Controls.Add(this.valMax);
            this.Controls.Add(this.sizeMax);
            this.Controls.Add(this.valMin);
            this.Controls.Add(this.sizeMin);
            this.Controls.Add(this.satMin);
            this.Controls.Add(this.hueMin);
            this.Controls.Add(this.trackSizeMax);
            this.Controls.Add(this.trackSizeMin);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.trackValMax);
            this.Controls.Add(this.trackSatMax);
            this.Controls.Add(this.trackHueMax);
            this.Controls.Add(this.trackValMin);
            this.Controls.Add(this.trackSatMin);
            this.Controls.Add(this.trackHueMin);
            this.Name = "AppX3";
            this.Text = "AppX3";
            ((System.ComponentModel.ISupportInitialize)(this.trackSizeMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackSizeMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackValMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackSatMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackHueMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackValMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackSatMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackHueMin)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar trackSizeMax;
        private System.Windows.Forms.TrackBar trackSizeMin;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar trackValMax;
        private System.Windows.Forms.TrackBar trackSatMax;
        private System.Windows.Forms.TrackBar trackHueMax;
        private System.Windows.Forms.TrackBar trackValMin;
        private System.Windows.Forms.TrackBar trackSatMin;
        private System.Windows.Forms.TrackBar trackHueMin;
        private System.Windows.Forms.Label hueMin;
        private System.Windows.Forms.Label satMin;
        private System.Windows.Forms.Label sizeMin;
        private System.Windows.Forms.Label valMin;
        private System.Windows.Forms.Label sizeMax;
        private System.Windows.Forms.Label valMax;
        private System.Windows.Forms.Label hueMax;
        private System.Windows.Forms.Label satMax;
    }
}