namespace OctaHarness
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
            this.txtMessageStatus = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btnSendAbePacket = new System.Windows.Forms.Button();
            this.txtMLon = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtMLat = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtMaxSpeed = new System.Windows.Forms.TextBox();
            this.lblDriverID = new System.Windows.Forms.Label();
            this.btnSendPacket = new System.Windows.Forms.Button();
            this.txtLon = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtLat = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtTime = new System.Windows.Forms.TextBox();
            this.txtSpeed = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtHead = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtVehicleStatus = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtIPAddress = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btnStartTimer = new System.Windows.Forms.Button();
            this.btnStopTimer = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtServiceAddress = new System.Windows.Forms.TextBox();
            this.btnSendState = new System.Windows.Forms.Button();
            this.btnSendAssistRequest = new System.Windows.Forms.Button();
            this.btnParseKML = new System.Windows.Forms.Button();
            this.btnStopPlay = new System.Windows.Forms.Button();
            this.btnClearStatus = new System.Windows.Forms.Button();
            this.btnPlayCSV = new System.Windows.Forms.Button();
            this.btnStopCSV = new System.Windows.Forms.Button();
            this.btnNewIncident = new System.Windows.Forms.Button();
            this.btnNewAssist = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.txtServiceRef = new System.Windows.Forms.TextBox();
            this.btnCurrentTrucks = new System.Windows.Forms.Button();
            this.btnSendMessage = new System.Windows.Forms.Button();
            this.btnAlarmCheck = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtMessageStatus
            // 
            this.txtMessageStatus.Location = new System.Drawing.Point(576, 60);
            this.txtMessageStatus.Margin = new System.Windows.Forms.Padding(6);
            this.txtMessageStatus.Multiline = true;
            this.txtMessageStatus.Name = "txtMessageStatus";
            this.txtMessageStatus.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMessageStatus.Size = new System.Drawing.Size(1024, 596);
            this.txtMessageStatus.TabIndex = 23;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(576, 23);
            this.label8.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(167, 25);
            this.label8.TabIndex = 24;
            this.label8.Text = "Message Status";
            // 
            // btnSendAbePacket
            // 
            this.btnSendAbePacket.Location = new System.Drawing.Point(34, 560);
            this.btnSendAbePacket.Margin = new System.Windows.Forms.Padding(6);
            this.btnSendAbePacket.Name = "btnSendAbePacket";
            this.btnSendAbePacket.Size = new System.Drawing.Size(530, 44);
            this.btnSendAbePacket.TabIndex = 53;
            this.btnSendAbePacket.Text = "Send Abe Packet";
            this.btnSendAbePacket.UseVisualStyleBackColor = true;
            this.btnSendAbePacket.Click += new System.EventHandler(this.btnSendAbePacket_Click);
            // 
            // txtMLon
            // 
            this.txtMLon.Location = new System.Drawing.Point(296, 273);
            this.txtMLon.Margin = new System.Windows.Forms.Padding(6);
            this.txtMLon.Name = "txtMLon";
            this.txtMLon.Size = new System.Drawing.Size(142, 31);
            this.txtMLon.TabIndex = 52;
            this.txtMLon.Text = "-42.909878";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(216, 279);
            this.label6.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(66, 25);
            this.label6.TabIndex = 51;
            this.label6.Text = "MLon";
            // 
            // txtMLat
            // 
            this.txtMLat.Location = new System.Drawing.Point(98, 273);
            this.txtMLat.Margin = new System.Windows.Forms.Padding(6);
            this.txtMLat.Name = "txtMLat";
            this.txtMLat.Size = new System.Drawing.Size(102, 31);
            this.txtMLat.TabIndex = 50;
            this.txtMLat.Text = "12.7866";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(28, 279);
            this.label11.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(60, 25);
            this.label11.TabIndex = 49;
            this.label11.Text = "MLat";
            // 
            // txtMaxSpeed
            // 
            this.txtMaxSpeed.Location = new System.Drawing.Point(214, 223);
            this.txtMaxSpeed.Margin = new System.Windows.Forms.Padding(6);
            this.txtMaxSpeed.Name = "txtMaxSpeed";
            this.txtMaxSpeed.Size = new System.Drawing.Size(346, 31);
            this.txtMaxSpeed.TabIndex = 48;
            this.txtMaxSpeed.Text = "65";
            // 
            // lblDriverID
            // 
            this.lblDriverID.AutoSize = true;
            this.lblDriverID.Location = new System.Drawing.Point(30, 229);
            this.lblDriverID.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblDriverID.Name = "lblDriverID";
            this.lblDriverID.Size = new System.Drawing.Size(121, 25);
            this.lblDriverID.TabIndex = 47;
            this.lblDriverID.Text = "Max Speed";
            // 
            // btnSendPacket
            // 
            this.btnSendPacket.Location = new System.Drawing.Point(32, 615);
            this.btnSendPacket.Margin = new System.Windows.Forms.Padding(6);
            this.btnSendPacket.Name = "btnSendPacket";
            this.btnSendPacket.Size = new System.Drawing.Size(252, 44);
            this.btnSendPacket.TabIndex = 46;
            this.btnSendPacket.Text = "&Send GPS Packet";
            this.btnSendPacket.UseVisualStyleBackColor = true;
            this.btnSendPacket.Click += new System.EventHandler(this.btnSendPacket_Click_1);
            // 
            // txtLon
            // 
            this.txtLon.Location = new System.Drawing.Point(278, 173);
            this.txtLon.Margin = new System.Windows.Forms.Padding(6);
            this.txtLon.Name = "txtLon";
            this.txtLon.Size = new System.Drawing.Size(142, 31);
            this.txtLon.TabIndex = 45;
            this.txtLon.Text = "-117.85245454410457";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(216, 179);
            this.label10.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(48, 25);
            this.label10.TabIndex = 44;
            this.label10.Text = "Lon";
            // 
            // txtLat
            // 
            this.txtLat.Location = new System.Drawing.Point(98, 173);
            this.txtLat.Margin = new System.Windows.Forms.Padding(6);
            this.txtLat.Name = "txtLat";
            this.txtLat.Size = new System.Drawing.Size(102, 31);
            this.txtLat.TabIndex = 43;
            this.txtLat.Text = "33.775141495952752";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(28, 179);
            this.label9.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(42, 25);
            this.label9.TabIndex = 42;
            this.label9.Text = "Lat";
            // 
            // txtTime
            // 
            this.txtTime.Location = new System.Drawing.Point(214, 323);
            this.txtTime.Margin = new System.Windows.Forms.Padding(6);
            this.txtTime.Name = "txtTime";
            this.txtTime.ReadOnly = true;
            this.txtTime.Size = new System.Drawing.Size(346, 31);
            this.txtTime.TabIndex = 41;
            // 
            // txtSpeed
            // 
            this.txtSpeed.Location = new System.Drawing.Point(214, 123);
            this.txtSpeed.Margin = new System.Windows.Forms.Padding(6);
            this.txtSpeed.Name = "txtSpeed";
            this.txtSpeed.Size = new System.Drawing.Size(346, 31);
            this.txtSpeed.TabIndex = 40;
            this.txtSpeed.Text = "55";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(28, 129);
            this.label7.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(74, 25);
            this.label7.TabIndex = 39;
            this.label7.Text = "Speed";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(30, 329);
            this.label5.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 25);
            this.label5.TabIndex = 38;
            this.label5.Text = "Time";
            // 
            // txtHead
            // 
            this.txtHead.Location = new System.Drawing.Point(214, 73);
            this.txtHead.Margin = new System.Windows.Forms.Padding(6);
            this.txtHead.Name = "txtHead";
            this.txtHead.Size = new System.Drawing.Size(346, 31);
            this.txtHead.TabIndex = 37;
            this.txtHead.Text = "331";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(30, 79);
            this.label4.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 25);
            this.label4.TabIndex = 36;
            this.label4.Text = "Head";
            // 
            // txtVehicleStatus
            // 
            this.txtVehicleStatus.Location = new System.Drawing.Point(202, 671);
            this.txtVehicleStatus.Margin = new System.Windows.Forms.Padding(6);
            this.txtVehicleStatus.Name = "txtVehicleStatus";
            this.txtVehicleStatus.Size = new System.Drawing.Size(346, 31);
            this.txtVehicleStatus.TabIndex = 35;
            this.txtVehicleStatus.Text = "On Assist";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 677);
            this.label3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 25);
            this.label3.TabIndex = 34;
            this.label3.Text = "Status";
            // 
            // txtIPAddress
            // 
            this.txtIPAddress.Location = new System.Drawing.Point(214, 23);
            this.txtIPAddress.Margin = new System.Windows.Forms.Padding(6);
            this.txtIPAddress.Name = "txtIPAddress";
            this.txtIPAddress.ReadOnly = true;
            this.txtIPAddress.Size = new System.Drawing.Size(346, 31);
            this.txtIPAddress.TabIndex = 33;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 29);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(116, 25);
            this.label1.TabIndex = 32;
            this.label1.Text = "IP Address";
            // 
            // timer1
            // 
            this.timer1.Interval = 60000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // btnStartTimer
            // 
            this.btnStartTimer.Location = new System.Drawing.Point(22, 715);
            this.btnStartTimer.Margin = new System.Windows.Forms.Padding(6);
            this.btnStartTimer.Name = "btnStartTimer";
            this.btnStartTimer.Size = new System.Drawing.Size(150, 44);
            this.btnStartTimer.TabIndex = 54;
            this.btnStartTimer.Text = "Start";
            this.btnStartTimer.UseVisualStyleBackColor = true;
            this.btnStartTimer.Click += new System.EventHandler(this.btnStartTimer_Click);
            // 
            // btnStopTimer
            // 
            this.btnStopTimer.Location = new System.Drawing.Point(186, 715);
            this.btnStopTimer.Margin = new System.Windows.Forms.Padding(6);
            this.btnStopTimer.Name = "btnStopTimer";
            this.btnStopTimer.Size = new System.Drawing.Size(150, 44);
            this.btnStopTimer.TabIndex = 55;
            this.btnStopTimer.Text = "Stop";
            this.btnStopTimer.UseVisualStyleBackColor = true;
            this.btnStopTimer.Click += new System.EventHandler(this.btnStopTimer_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(30, 379);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(169, 25);
            this.label2.TabIndex = 56;
            this.label2.Text = "Service Address";
            // 
            // txtServiceAddress
            // 
            this.txtServiceAddress.Location = new System.Drawing.Point(214, 373);
            this.txtServiceAddress.Margin = new System.Windows.Forms.Padding(6);
            this.txtServiceAddress.Name = "txtServiceAddress";
            this.txtServiceAddress.Size = new System.Drawing.Size(344, 31);
            this.txtServiceAddress.TabIndex = 57;
            this.txtServiceAddress.Text = "38.124.164.211";
            // 
            // btnSendState
            // 
            this.btnSendState.Location = new System.Drawing.Point(34, 504);
            this.btnSendState.Margin = new System.Windows.Forms.Padding(6);
            this.btnSendState.Name = "btnSendState";
            this.btnSendState.Size = new System.Drawing.Size(530, 44);
            this.btnSendState.TabIndex = 58;
            this.btnSendState.Text = "Send State";
            this.btnSendState.UseVisualStyleBackColor = true;
            this.btnSendState.Click += new System.EventHandler(this.btnSendState_Click);
            // 
            // btnSendAssistRequest
            // 
            this.btnSendAssistRequest.Location = new System.Drawing.Point(348, 715);
            this.btnSendAssistRequest.Margin = new System.Windows.Forms.Padding(6);
            this.btnSendAssistRequest.Name = "btnSendAssistRequest";
            this.btnSendAssistRequest.Size = new System.Drawing.Size(204, 44);
            this.btnSendAssistRequest.TabIndex = 59;
            this.btnSendAssistRequest.Text = "Assist Request";
            this.btnSendAssistRequest.UseVisualStyleBackColor = true;
            this.btnSendAssistRequest.Click += new System.EventHandler(this.btnSendAssistRequest_Click);
            // 
            // btnParseKML
            // 
            this.btnParseKML.Location = new System.Drawing.Point(582, 663);
            this.btnParseKML.Margin = new System.Windows.Forms.Padding(6);
            this.btnParseKML.Name = "btnParseKML";
            this.btnParseKML.Size = new System.Drawing.Size(150, 44);
            this.btnParseKML.TabIndex = 60;
            this.btnParseKML.Text = "Play KML";
            this.btnParseKML.UseVisualStyleBackColor = true;
            this.btnParseKML.Click += new System.EventHandler(this.btnParseKML_Click);
            // 
            // btnStopPlay
            // 
            this.btnStopPlay.Location = new System.Drawing.Point(744, 663);
            this.btnStopPlay.Margin = new System.Windows.Forms.Padding(6);
            this.btnStopPlay.Name = "btnStopPlay";
            this.btnStopPlay.Size = new System.Drawing.Size(150, 44);
            this.btnStopPlay.TabIndex = 61;
            this.btnStopPlay.Text = "Stop Play";
            this.btnStopPlay.UseVisualStyleBackColor = true;
            this.btnStopPlay.Click += new System.EventHandler(this.btnStopPlay_Click);
            // 
            // btnClearStatus
            // 
            this.btnClearStatus.Location = new System.Drawing.Point(908, 663);
            this.btnClearStatus.Margin = new System.Windows.Forms.Padding(6);
            this.btnClearStatus.Name = "btnClearStatus";
            this.btnClearStatus.Size = new System.Drawing.Size(150, 44);
            this.btnClearStatus.TabIndex = 62;
            this.btnClearStatus.Text = "Clear Status";
            this.btnClearStatus.UseVisualStyleBackColor = true;
            this.btnClearStatus.Click += new System.EventHandler(this.btnClearStatus_Click);
            // 
            // btnPlayCSV
            // 
            this.btnPlayCSV.Location = new System.Drawing.Point(1070, 663);
            this.btnPlayCSV.Margin = new System.Windows.Forms.Padding(6);
            this.btnPlayCSV.Name = "btnPlayCSV";
            this.btnPlayCSV.Size = new System.Drawing.Size(150, 44);
            this.btnPlayCSV.TabIndex = 63;
            this.btnPlayCSV.Text = "Play CSV";
            this.btnPlayCSV.UseVisualStyleBackColor = true;
            this.btnPlayCSV.Click += new System.EventHandler(this.btnPlayCSV_Click);
            // 
            // btnStopCSV
            // 
            this.btnStopCSV.Location = new System.Drawing.Point(1232, 663);
            this.btnStopCSV.Margin = new System.Windows.Forms.Padding(6);
            this.btnStopCSV.Name = "btnStopCSV";
            this.btnStopCSV.Size = new System.Drawing.Size(150, 44);
            this.btnStopCSV.TabIndex = 64;
            this.btnStopCSV.Text = "Stop CSV";
            this.btnStopCSV.UseVisualStyleBackColor = true;
            this.btnStopCSV.Click += new System.EventHandler(this.btnStopCSV_Click);
            // 
            // btnNewIncident
            // 
            this.btnNewIncident.Location = new System.Drawing.Point(582, 713);
            this.btnNewIncident.Margin = new System.Windows.Forms.Padding(6);
            this.btnNewIncident.Name = "btnNewIncident";
            this.btnNewIncident.Size = new System.Drawing.Size(230, 44);
            this.btnNewIncident.TabIndex = 65;
            this.btnNewIncident.Text = "Incidents";
            this.btnNewIncident.UseVisualStyleBackColor = true;
            this.btnNewIncident.Click += new System.EventHandler(this.btnNewIncident_Click);
            // 
            // btnNewAssist
            // 
            this.btnNewAssist.Location = new System.Drawing.Point(828, 712);
            this.btnNewAssist.Margin = new System.Windows.Forms.Padding(6);
            this.btnNewAssist.Name = "btnNewAssist";
            this.btnNewAssist.Size = new System.Drawing.Size(230, 44);
            this.btnNewAssist.TabIndex = 67;
            this.btnNewAssist.Text = "Assists";
            this.btnNewAssist.UseVisualStyleBackColor = true;
            this.btnNewAssist.Click += new System.EventHandler(this.btnNewAssist_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(30, 429);
            this.label12.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(45, 25);
            this.label12.TabIndex = 68;
            this.label12.Text = "Ref";
            // 
            // txtServiceRef
            // 
            this.txtServiceRef.Location = new System.Drawing.Point(90, 423);
            this.txtServiceRef.Margin = new System.Windows.Forms.Padding(6);
            this.txtServiceRef.Name = "txtServiceRef";
            this.txtServiceRef.Size = new System.Drawing.Size(468, 31);
            this.txtServiceRef.TabIndex = 69;
            this.txtServiceRef.Text = "38.124.164.211:9007/TowTruckService.svc";
            // 
            // btnCurrentTrucks
            // 
            this.btnCurrentTrucks.Location = new System.Drawing.Point(1072, 712);
            this.btnCurrentTrucks.Margin = new System.Windows.Forms.Padding(6);
            this.btnCurrentTrucks.Name = "btnCurrentTrucks";
            this.btnCurrentTrucks.Size = new System.Drawing.Size(310, 44);
            this.btnCurrentTrucks.TabIndex = 70;
            this.btnCurrentTrucks.Text = "Current Trucks";
            this.btnCurrentTrucks.UseVisualStyleBackColor = true;
            this.btnCurrentTrucks.Click += new System.EventHandler(this.btnCurrentTrucks_Click);
            // 
            // btnSendMessage
            // 
            this.btnSendMessage.Location = new System.Drawing.Point(1394, 712);
            this.btnSendMessage.Margin = new System.Windows.Forms.Padding(6);
            this.btnSendMessage.Name = "btnSendMessage";
            this.btnSendMessage.Size = new System.Drawing.Size(194, 44);
            this.btnSendMessage.TabIndex = 71;
            this.btnSendMessage.Text = "Send Message";
            this.btnSendMessage.UseVisualStyleBackColor = true;
            this.btnSendMessage.Click += new System.EventHandler(this.btnSendMessage_Click);
            // 
            // btnAlarmCheck
            // 
            this.btnAlarmCheck.Location = new System.Drawing.Point(1394, 663);
            this.btnAlarmCheck.Margin = new System.Windows.Forms.Padding(6);
            this.btnAlarmCheck.Name = "btnAlarmCheck";
            this.btnAlarmCheck.Size = new System.Drawing.Size(150, 44);
            this.btnAlarmCheck.TabIndex = 72;
            this.btnAlarmCheck.Text = "Alarm Chk";
            this.btnAlarmCheck.UseVisualStyleBackColor = true;
            this.btnAlarmCheck.Click += new System.EventHandler(this.btnAlarmCheck_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(310, 615);
            this.button1.Margin = new System.Windows.Forms.Padding(6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(252, 44);
            this.button1.TabIndex = 73;
            this.button1.Text = "FWD GPS Packet";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1612, 779);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnAlarmCheck);
            this.Controls.Add(this.btnSendMessage);
            this.Controls.Add(this.btnCurrentTrucks);
            this.Controls.Add(this.txtServiceRef);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.btnNewAssist);
            this.Controls.Add(this.btnNewIncident);
            this.Controls.Add(this.btnStopCSV);
            this.Controls.Add(this.btnPlayCSV);
            this.Controls.Add(this.btnClearStatus);
            this.Controls.Add(this.btnStopPlay);
            this.Controls.Add(this.btnParseKML);
            this.Controls.Add(this.btnSendAssistRequest);
            this.Controls.Add(this.btnSendState);
            this.Controls.Add(this.txtServiceAddress);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnStopTimer);
            this.Controls.Add(this.btnStartTimer);
            this.Controls.Add(this.btnSendAbePacket);
            this.Controls.Add(this.txtMLon);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtMLat);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.txtMaxSpeed);
            this.Controls.Add(this.lblDriverID);
            this.Controls.Add(this.btnSendPacket);
            this.Controls.Add(this.txtLon);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.txtLat);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.txtTime);
            this.Controls.Add(this.txtSpeed);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtHead);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtVehicleStatus);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtIPAddress);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtMessageStatus);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "Form1";
            this.Text = "OCTA Test Harness";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtMessageStatus;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnSendAbePacket;
        private System.Windows.Forms.TextBox txtMLon;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtMLat;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtMaxSpeed;
        private System.Windows.Forms.Label lblDriverID;
        private System.Windows.Forms.Button btnSendPacket;
        private System.Windows.Forms.TextBox txtLon;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtLat;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtTime;
        private System.Windows.Forms.TextBox txtSpeed;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtHead;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtVehicleStatus;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtIPAddress;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button btnStartTimer;
        private System.Windows.Forms.Button btnStopTimer;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtServiceAddress;
        private System.Windows.Forms.Button btnSendState;
        private System.Windows.Forms.Button btnSendAssistRequest;
        private System.Windows.Forms.Button btnParseKML;
        private System.Windows.Forms.Button btnStopPlay;
        private System.Windows.Forms.Button btnClearStatus;
        private System.Windows.Forms.Button btnPlayCSV;
        private System.Windows.Forms.Button btnStopCSV;
        private System.Windows.Forms.Button btnNewIncident;
        private System.Windows.Forms.Button btnNewAssist;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtServiceRef;
        private System.Windows.Forms.Button btnCurrentTrucks;
        private System.Windows.Forms.Button btnSendMessage;
        private System.Windows.Forms.Button btnAlarmCheck;
        private System.Windows.Forms.Button button1;
    }
}

