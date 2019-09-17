﻿namespace FSP_PlayBack
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnLoadPlayback = new System.Windows.Forms.Button();
            this.chkLogon = new System.Windows.Forms.CheckBox();
            this.cboTrucks = new System.Windows.Forms.ComboBox();
            this.lblTruck = new System.Windows.Forms.Label();
            this.cboBeats = new System.Windows.Forms.ComboBox();
            this.lblBeatSelect = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnLoadBeatsAndTrucks = new System.Windows.Forms.Button();
            this.timePickerTo = new System.Windows.Forms.DateTimePicker();
            this.timePickerFrom = new System.Windows.Forms.DateTimePicker();
            this.datePickerTo = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.datePickerFrom = new System.Windows.Forms.DateTimePicker();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.gvData = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvData)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.SystemColors.Window;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.AutoScroll = true;
            this.splitContainer1.Panel1.BackColor = System.Drawing.Color.Gray;
            this.splitContainer1.Panel1.Controls.Add(this.groupBox2);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel1MinSize = 0;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Panel2MinSize = 0;
            this.splitContainer1.Size = new System.Drawing.Size(2140, 1454);
            this.splitContainer1.SplitterDistance = 534;
            this.splitContainer1.SplitterWidth = 10;
            this.splitContainer1.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.Transparent;
            this.groupBox2.Controls.Add(this.btnLoadPlayback);
            this.groupBox2.Controls.Add(this.chkLogon);
            this.groupBox2.Controls.Add(this.cboTrucks);
            this.groupBox2.Controls.Add(this.lblTruck);
            this.groupBox2.Controls.Add(this.cboBeats);
            this.groupBox2.Controls.Add(this.lblBeatSelect);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.857143F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(15, 336);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(490, 258);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Select:";
            // 
            // btnLoadPlayback
            // 
            this.btnLoadPlayback.Location = new System.Drawing.Point(13, 166);
            this.btnLoadPlayback.Name = "btnLoadPlayback";
            this.btnLoadPlayback.Size = new System.Drawing.Size(450, 51);
            this.btnLoadPlayback.TabIndex = 8;
            this.btnLoadPlayback.Text = "Load Playback";
            this.btnLoadPlayback.UseVisualStyleBackColor = true;
            this.btnLoadPlayback.Click += new System.EventHandler(this.btnLoadPlayback_Click);
            // 
            // chkLogon
            // 
            this.chkLogon.AutoSize = true;
            this.chkLogon.Location = new System.Drawing.Point(13, 109);
            this.chkLogon.Name = "chkLogon";
            this.chkLogon.Size = new System.Drawing.Size(360, 33);
            this.chkLogon.TabIndex = 11;
            this.chkLogon.Text = "Show Only Logged On Events";
            this.chkLogon.UseVisualStyleBackColor = true;
            // 
            // cboTrucks
            // 
            this.cboTrucks.FormattingEnabled = true;
            this.cboTrucks.Location = new System.Drawing.Point(89, 42);
            this.cboTrucks.Name = "cboTrucks";
            this.cboTrucks.Size = new System.Drawing.Size(142, 37);
            this.cboTrucks.TabIndex = 10;
            // 
            // lblTruck
            // 
            this.lblTruck.AutoSize = true;
            this.lblTruck.Location = new System.Drawing.Point(8, 44);
            this.lblTruck.Name = "lblTruck";
            this.lblTruck.Size = new System.Drawing.Size(80, 29);
            this.lblTruck.TabIndex = 9;
            this.lblTruck.Text = "Truck:";
            // 
            // cboBeats
            // 
            this.cboBeats.FormattingEnabled = true;
            this.cboBeats.Location = new System.Drawing.Point(321, 43);
            this.cboBeats.Name = "cboBeats";
            this.cboBeats.Size = new System.Drawing.Size(142, 37);
            this.cboBeats.TabIndex = 0;
            // 
            // lblBeatSelect
            // 
            this.lblBeatSelect.AutoSize = true;
            this.lblBeatSelect.Location = new System.Drawing.Point(252, 45);
            this.lblBeatSelect.Name = "lblBeatSelect";
            this.lblBeatSelect.Size = new System.Drawing.Size(68, 29);
            this.lblBeatSelect.TabIndex = 8;
            this.lblBeatSelect.Text = "Beat:";
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.btnLoadBeatsAndTrucks);
            this.groupBox1.Controls.Add(this.timePickerTo);
            this.groupBox1.Controls.Add(this.timePickerFrom);
            this.groupBox1.Controls.Add(this.datePickerTo);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.datePickerFrom);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.857143F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(14, 11);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(490, 309);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Search Date/Time Range:";
            // 
            // btnLoadBeatsAndTrucks
            // 
            this.btnLoadBeatsAndTrucks.Location = new System.Drawing.Point(27, 226);
            this.btnLoadBeatsAndTrucks.Name = "btnLoadBeatsAndTrucks";
            this.btnLoadBeatsAndTrucks.Size = new System.Drawing.Size(435, 51);
            this.btnLoadBeatsAndTrucks.TabIndex = 7;
            this.btnLoadBeatsAndTrucks.Text = "Load Beats and Trucks";
            this.btnLoadBeatsAndTrucks.UseVisualStyleBackColor = true;
            this.btnLoadBeatsAndTrucks.Click += new System.EventHandler(this.BtnLoadBeatsAndTrucks_Click);
            // 
            // timePickerTo
            // 
            this.timePickerTo.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.timePickerTo.Location = new System.Drawing.Point(247, 163);
            this.timePickerTo.Name = "timePickerTo";
            this.timePickerTo.ShowUpDown = true;
            this.timePickerTo.Size = new System.Drawing.Size(215, 34);
            this.timePickerTo.TabIndex = 6;
            // 
            // timePickerFrom
            // 
            this.timePickerFrom.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.timePickerFrom.Location = new System.Drawing.Point(248, 74);
            this.timePickerFrom.Name = "timePickerFrom";
            this.timePickerFrom.ShowUpDown = true;
            this.timePickerFrom.Size = new System.Drawing.Size(214, 34);
            this.timePickerFrom.TabIndex = 5;
            // 
            // datePickerTo
            // 
            this.datePickerTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.datePickerTo.Location = new System.Drawing.Point(27, 163);
            this.datePickerTo.Name = "datePickerTo";
            this.datePickerTo.Size = new System.Drawing.Size(215, 34);
            this.datePickerTo.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(31, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 29);
            this.label2.TabIndex = 1;
            this.label2.Text = "From:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(27, 128);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 29);
            this.label3.TabIndex = 3;
            this.label3.Text = "To:";
            // 
            // datePickerFrom
            // 
            this.datePickerFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.datePickerFrom.Location = new System.Drawing.Point(27, 74);
            this.datePickerFrom.Name = "datePickerFrom";
            this.datePickerFrom.Size = new System.Drawing.Size(215, 34);
            this.datePickerFrom.TabIndex = 2;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.gvData);
            this.splitContainer2.Size = new System.Drawing.Size(1596, 1454);
            this.splitContainer2.SplitterDistance = 844;
            this.splitContainer2.TabIndex = 0;
            // 
            // gvData
            // 
            this.gvData.AllowUserToAddRows = false;
            this.gvData.AllowUserToDeleteRows = false;
            this.gvData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gvData.Location = new System.Drawing.Point(0, 0);
            this.gvData.Name = "gvData";
            this.gvData.ReadOnly = true;
            this.gvData.RowHeadersWidth = 72;
            this.gvData.RowTemplate.Height = 31;
            this.gvData.Size = new System.Drawing.Size(1596, 606);
            this.gvData.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2140, 1454);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gvData)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DateTimePicker datePickerTo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker datePickerFrom;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox cboBeats;
        private System.Windows.Forms.DateTimePicker timePickerFrom;
        private System.Windows.Forms.DateTimePicker timePickerTo;
        private System.Windows.Forms.Button btnLoadBeatsAndTrucks;
        private System.Windows.Forms.Label lblBeatSelect;
        private System.Windows.Forms.ComboBox cboTrucks;
        private System.Windows.Forms.Label lblTruck;
        private System.Windows.Forms.CheckBox chkLogon;
        private System.Windows.Forms.Button btnLoadPlayback;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.DataGridView gvData;
    }
}

