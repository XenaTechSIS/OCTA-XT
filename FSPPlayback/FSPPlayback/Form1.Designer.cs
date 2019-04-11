namespace FSPPlayback
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.gMapControl1 = new GMap.NET.WindowsForms.GMapControl();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.chkLeaveTrail = new System.Windows.Forms.CheckBox();
            this.cboBeats = new System.Windows.Forms.ComboBox();
            this.lblSelectBeat = new System.Windows.Forms.Label();
            this.cboPlaybackSpeed = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnStopPlayback = new System.Windows.Forms.Button();
            this.btnStartPlayback = new System.Windows.Forms.Button();
            this.txtEndHour = new System.Windows.Forms.TextBox();
            this.txtStartHour = new System.Windows.Forms.TextBox();
            this.chkLogon = new System.Windows.Forms.CheckBox();
            this.cboTrucks = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnLoadTrucks = new System.Windows.Forms.Button();
            this.lblStart = new System.Windows.Forms.Label();
            this.lblEnd = new System.Windows.Forms.Label();
            this.dtpStart = new System.Windows.Forms.DateTimePicker();
            this.dtpEnd = new System.Windows.Forms.DateTimePicker();
            this.btnLoadPlayback = new System.Windows.Forms.Button();
            this.gvData = new System.Windows.Forms.DataGridView();
            this.tmrPlayback = new System.Windows.Forms.Timer(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvData)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.gMapControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(2376, 1208);
            this.splitContainer1.SplitterDistance = 802;
            this.splitContainer1.SplitterWidth = 8;
            this.splitContainer1.TabIndex = 0;
            // 
            // gMapControl1
            // 
            this.gMapControl1.Bearing = 0F;
            this.gMapControl1.CanDragMap = true;
            this.gMapControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gMapControl1.EmptyTileColor = System.Drawing.Color.Navy;
            this.gMapControl1.GrayScaleMode = false;
            this.gMapControl1.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.gMapControl1.LevelsKeepInMemmory = 5;
            this.gMapControl1.Location = new System.Drawing.Point(0, 0);
            this.gMapControl1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.gMapControl1.MarkersEnabled = true;
            this.gMapControl1.MaxZoom = 18;
            this.gMapControl1.MinZoom = 2;
            this.gMapControl1.MouseWheelZoomEnabled = true;
            this.gMapControl1.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            this.gMapControl1.Name = "gMapControl1";
            this.gMapControl1.NegativeMode = false;
            this.gMapControl1.PolygonsEnabled = true;
            this.gMapControl1.RetryLoadTile = 0;
            this.gMapControl1.RoutesEnabled = true;
            this.gMapControl1.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Integer;
            this.gMapControl1.SelectedAreaFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(65)))), ((int)(((byte)(105)))), ((int)(((byte)(225)))));
            this.gMapControl1.ShowTileGridLines = false;
            this.gMapControl1.Size = new System.Drawing.Size(2376, 802);
            this.gMapControl1.TabIndex = 0;
            this.gMapControl1.Zoom = 15D;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.chkLeaveTrail);
            this.splitContainer2.Panel1.Controls.Add(this.cboBeats);
            this.splitContainer2.Panel1.Controls.Add(this.lblSelectBeat);
            this.splitContainer2.Panel1.Controls.Add(this.cboPlaybackSpeed);
            this.splitContainer2.Panel1.Controls.Add(this.label2);
            this.splitContainer2.Panel1.Controls.Add(this.btnStopPlayback);
            this.splitContainer2.Panel1.Controls.Add(this.btnStartPlayback);
            this.splitContainer2.Panel1.Controls.Add(this.txtEndHour);
            this.splitContainer2.Panel1.Controls.Add(this.txtStartHour);
            this.splitContainer2.Panel1.Controls.Add(this.chkLogon);
            this.splitContainer2.Panel1.Controls.Add(this.cboTrucks);
            this.splitContainer2.Panel1.Controls.Add(this.label1);
            this.splitContainer2.Panel1.Controls.Add(this.btnLoadTrucks);
            this.splitContainer2.Panel1.Controls.Add(this.lblStart);
            this.splitContainer2.Panel1.Controls.Add(this.lblEnd);
            this.splitContainer2.Panel1.Controls.Add(this.dtpStart);
            this.splitContainer2.Panel1.Controls.Add(this.dtpEnd);
            this.splitContainer2.Panel1.Controls.Add(this.btnLoadPlayback);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.gvData);
            this.splitContainer2.Size = new System.Drawing.Size(2376, 398);
            this.splitContainer2.SplitterDistance = 692;
            this.splitContainer2.SplitterWidth = 8;
            this.splitContainer2.TabIndex = 16;
            // 
            // chkLeaveTrail
            // 
            this.chkLeaveTrail.AutoSize = true;
            this.chkLeaveTrail.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkLeaveTrail.Location = new System.Drawing.Point(26, 308);
            this.chkLeaveTrail.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.chkLeaveTrail.Name = "chkLeaveTrail";
            this.chkLeaveTrail.Size = new System.Drawing.Size(151, 29);
            this.chkLeaveTrail.TabIndex = 33;
            this.chkLeaveTrail.Text = "Leave Trail";
            this.chkLeaveTrail.UseVisualStyleBackColor = true;
            // 
            // cboBeats
            // 
            this.cboBeats.FormattingEnabled = true;
            this.cboBeats.Location = new System.Drawing.Point(486, 160);
            this.cboBeats.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.cboBeats.Name = "cboBeats";
            this.cboBeats.Size = new System.Drawing.Size(178, 33);
            this.cboBeats.TabIndex = 32;
            this.toolTip1.SetToolTip(this.cboBeats, "Select data by beat, ignored if truck is selected");
            // 
            // lblSelectBeat
            // 
            this.lblSelectBeat.AutoSize = true;
            this.lblSelectBeat.Location = new System.Drawing.Point(350, 165);
            this.lblSelectBeat.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblSelectBeat.Name = "lblSelectBeat";
            this.lblSelectBeat.Size = new System.Drawing.Size(122, 25);
            this.lblSelectBeat.TabIndex = 31;
            this.lblSelectBeat.Text = "Select Beat";
            // 
            // cboPlaybackSpeed
            // 
            this.cboPlaybackSpeed.FormattingEnabled = true;
            this.cboPlaybackSpeed.Items.AddRange(new object[] {
            "1x",
            "2x",
            "3x",
            "4x",
            "5x",
            "10x",
            "20x",
            "50x"});
            this.cboPlaybackSpeed.Location = new System.Drawing.Point(532, 210);
            this.cboPlaybackSpeed.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.cboPlaybackSpeed.Name = "cboPlaybackSpeed";
            this.cboPlaybackSpeed.Size = new System.Drawing.Size(132, 33);
            this.cboPlaybackSpeed.TabIndex = 30;
            this.cboPlaybackSpeed.Text = "Select";
            this.toolTip1.SetToolTip(this.cboPlaybackSpeed, "Increase the speed of the playback");
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(350, 215);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(168, 25);
            this.label2.TabIndex = 29;
            this.label2.Text = "Playback Speed";
            // 
            // btnStopPlayback
            // 
            this.btnStopPlayback.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStopPlayback.Location = new System.Drawing.Point(350, 344);
            this.btnStopPlayback.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btnStopPlayback.Name = "btnStopPlayback";
            this.btnStopPlayback.Size = new System.Drawing.Size(318, 44);
            this.btnStopPlayback.TabIndex = 28;
            this.btnStopPlayback.Text = "Stop Playback";
            this.toolTip1.SetToolTip(this.btnStopPlayback, "Stop the playback at the current record, you can restart from the same point");
            this.btnStopPlayback.UseVisualStyleBackColor = true;
            this.btnStopPlayback.Click += new System.EventHandler(this.btnStopPlayback_Click_1);
            // 
            // btnStartPlayback
            // 
            this.btnStartPlayback.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStartPlayback.Location = new System.Drawing.Point(24, 344);
            this.btnStartPlayback.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btnStartPlayback.Name = "btnStartPlayback";
            this.btnStartPlayback.Size = new System.Drawing.Size(314, 44);
            this.btnStartPlayback.TabIndex = 27;
            this.btnStartPlayback.Text = "Start Playback";
            this.toolTip1.SetToolTip(this.btnStartPlayback, "Start playback on either on first record or selected record.");
            this.btnStartPlayback.UseVisualStyleBackColor = true;
            this.btnStartPlayback.Click += new System.EventHandler(this.btnStartPlayback_Click_1);
            // 
            // txtEndHour
            // 
            this.txtEndHour.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtEndHour.Location = new System.Drawing.Point(568, 58);
            this.txtEndHour.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.txtEndHour.Name = "txtEndHour";
            this.txtEndHour.Size = new System.Drawing.Size(98, 31);
            this.txtEndHour.TabIndex = 26;
            this.txtEndHour.Text = "23:59:59";
            this.toolTip1.SetToolTip(this.txtEndHour, "Ending Time (24 hour time)");
            // 
            // txtStartHour
            // 
            this.txtStartHour.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtStartHour.Location = new System.Drawing.Point(566, 8);
            this.txtStartHour.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.txtStartHour.Name = "txtStartHour";
            this.txtStartHour.Size = new System.Drawing.Size(100, 31);
            this.txtStartHour.TabIndex = 25;
            this.txtStartHour.Text = "00:00:00";
            this.toolTip1.SetToolTip(this.txtStartHour, "Starting Time (24 hour time)");
            // 
            // chkLogon
            // 
            this.chkLogon.AutoSize = true;
            this.chkLogon.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkLogon.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkLogon.Location = new System.Drawing.Point(24, 212);
            this.chkLogon.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.chkLogon.Name = "chkLogon";
            this.chkLogon.Size = new System.Drawing.Size(310, 29);
            this.chkLogon.TabIndex = 23;
            this.chkLogon.Text = "Only show logged on events";
            this.chkLogon.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.chkLogon, "Only disply data where the driver is logged on");
            this.chkLogon.UseVisualStyleBackColor = true;
            // 
            // cboTrucks
            // 
            this.cboTrucks.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboTrucks.FormattingEnabled = true;
            this.cboTrucks.Location = new System.Drawing.Point(154, 160);
            this.cboTrucks.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.cboTrucks.Name = "cboTrucks";
            this.cboTrucks.Size = new System.Drawing.Size(180, 33);
            this.cboTrucks.TabIndex = 22;
            this.toolTip1.SetToolTip(this.cboTrucks, "If truck is selected, beat is ignored");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 165);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(132, 25);
            this.label1.TabIndex = 21;
            this.label1.Text = "Select Truck";
            // 
            // btnLoadTrucks
            // 
            this.btnLoadTrucks.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoadTrucks.Location = new System.Drawing.Point(24, 102);
            this.btnLoadTrucks.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btnLoadTrucks.Name = "btnLoadTrucks";
            this.btnLoadTrucks.Size = new System.Drawing.Size(644, 44);
            this.btnLoadTrucks.TabIndex = 20;
            this.btnLoadTrucks.Text = "Load Trucks and Beats";
            this.toolTip1.SetToolTip(this.btnLoadTrucks, "Load up data in the selected start and end ranges");
            this.btnLoadTrucks.UseVisualStyleBackColor = true;
            this.btnLoadTrucks.Click += new System.EventHandler(this.btnLoadTrucks_Click_1);
            // 
            // lblStart
            // 
            this.lblStart.AutoSize = true;
            this.lblStart.Location = new System.Drawing.Point(18, 19);
            this.lblStart.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblStart.Name = "lblStart";
            this.lblStart.Size = new System.Drawing.Size(123, 25);
            this.lblStart.TabIndex = 16;
            this.lblStart.Text = "Select Start";
            // 
            // lblEnd
            // 
            this.lblEnd.AutoSize = true;
            this.lblEnd.Location = new System.Drawing.Point(18, 69);
            this.lblEnd.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblEnd.Name = "lblEnd";
            this.lblEnd.Size = new System.Drawing.Size(116, 25);
            this.lblEnd.TabIndex = 19;
            this.lblEnd.Text = "Select End";
            // 
            // dtpStart
            // 
            this.dtpStart.Location = new System.Drawing.Point(154, 8);
            this.dtpStart.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.dtpStart.Name = "dtpStart";
            this.dtpStart.Size = new System.Drawing.Size(396, 31);
            this.dtpStart.TabIndex = 17;
            this.toolTip1.SetToolTip(this.dtpStart, "Starting Date");
            // 
            // dtpEnd
            // 
            this.dtpEnd.Location = new System.Drawing.Point(154, 58);
            this.dtpEnd.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.dtpEnd.Name = "dtpEnd";
            this.dtpEnd.Size = new System.Drawing.Size(396, 31);
            this.dtpEnd.TabIndex = 18;
            this.toolTip1.SetToolTip(this.dtpEnd, "Ending Date");
            // 
            // btnLoadPlayback
            // 
            this.btnLoadPlayback.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoadPlayback.Location = new System.Drawing.Point(24, 256);
            this.btnLoadPlayback.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btnLoadPlayback.Name = "btnLoadPlayback";
            this.btnLoadPlayback.Size = new System.Drawing.Size(644, 44);
            this.btnLoadPlayback.TabIndex = 24;
            this.btnLoadPlayback.Text = "Load Playback";
            this.toolTip1.SetToolTip(this.btnLoadPlayback, "Load up the selected playback data for the selected date range and selected truck" +
        " or beat");
            this.btnLoadPlayback.UseVisualStyleBackColor = true;
            this.btnLoadPlayback.Click += new System.EventHandler(this.btnLoadPlayback_Click_1);
            // 
            // gvData
            // 
            this.gvData.CausesValidation = false;
            this.gvData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gvData.Location = new System.Drawing.Point(0, 0);
            this.gvData.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.gvData.Name = "gvData";
            this.gvData.Size = new System.Drawing.Size(1676, 398);
            this.gvData.TabIndex = 10;
            // 
            // tmrPlayback
            // 
            this.tmrPlayback.Interval = 30000;
            this.tmrPlayback.Tick += new System.EventHandler(this.tmrPlayback_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2376, 1208);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Name = "Form1";
            this.Text = "FSP Playback";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gvData)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private GMap.NET.WindowsForms.GMapControl gMapControl1;
        private System.Windows.Forms.Timer tmrPlayback;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ComboBox cboPlaybackSpeed;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnStopPlayback;
        private System.Windows.Forms.Button btnStartPlayback;
        private System.Windows.Forms.TextBox txtEndHour;
        private System.Windows.Forms.TextBox txtStartHour;
        private System.Windows.Forms.CheckBox chkLogon;
        private System.Windows.Forms.ComboBox cboTrucks;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnLoadTrucks;
        private System.Windows.Forms.Label lblStart;
        private System.Windows.Forms.Label lblEnd;
        private System.Windows.Forms.DateTimePicker dtpStart;
        private System.Windows.Forms.DateTimePicker dtpEnd;
        private System.Windows.Forms.Button btnLoadPlayback;
        private System.Windows.Forms.DataGridView gvData;
        private System.Windows.Forms.ComboBox cboBeats;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label lblSelectBeat;
        private System.Windows.Forms.CheckBox chkLeaveTrail;
    }
}

