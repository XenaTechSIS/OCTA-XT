<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TruckAlarmStatus.aspx.cs" Inherits="FPSService.Admin.TruckAlarmStatus" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Vehicle Alarm Status</title>
    <link href="../css/SiteStyle.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div id="Title"><h1>Vehicle Alarm Status&nbsp;<asp:Label ID="lblTruck" value="truck num" runat="server" /></h1></div>
    <asp:HyperLink ID="hlDashboard" Text="Dashboard" NavigateUrl="~/Admin/Dashboard.aspx" runat="server"/>
    <div id="alarms">
        <div><span class="lbl">Vehicle Number</span><span class="txt"><asp:Label ID="lblVehicleNumber" runat="server" value="val"></asp:Label></span></div>
        <div><span class="lbl">Driver Name</span><span class="txt"><asp:Label ID="lblDriverName" runat="server" value="val"></asp:Label></span></div>
        <div><span class="lbl">Contract Company Name</span><span class="txt"><asp:Label ID="lblContractCompanyName" runat="server" value="val"></asp:Label></span></div>
        <div><span class="lbl">Vehicle Status</span><span class="txt"><asp:Label ID="lblVehicleStatus" runat="server" value="val"></asp:Label></span></div>
        <div><span class="lbl">Status Started</span><span class="txt"><asp:Label ID="lblStatusStarted" runat="server" value="val"></asp:Label></span></div>
        <div><span class="lbl">Speeding Alarm</span><span class="txt"><asp:Label ID="lblSpeedingAlarm" runat="server" value="val"></asp:Label></span></div>
        <div><span class="lbl">Speeding Value</span><span class="txt"><asp:Label ID="lblSpeedingValue" runat="server" value="val"></asp:Label></span></div>
        <div><span class="lbl">Speeding Time</span><span class="txt"><asp:Label ID="lblSpeedingTime" runat="server" value="val"></asp:Label></span></div>
        <div><span class="lbl">Out of Bounds Alarm</span><span class="txt"><asp:Label ID="lblOutOfBoundsAlarm" runat="server" value="val"></asp:Label></span></div>
        <div><span class="lbl">Out of Bounds Message</span><span class="txt"><asp:Label ID="lblOutOfBoundsMessage" runat="server" value="val"></asp:Label></span></div>
        <div><span class="lbl">Out of Bounds Time</span><span class="txt"><asp:Label ID="lblOutOfBoundsTime" runat="server" value="val"></asp:Label></span></div>
        <div><span class="lbl">Out of Bounds Start Time</span><span class="txt"><asp:Label ID="lblOutOfBoundsStartTime" runat="server" value="val"></asp:Label></span></div>
        <div><span class="lbl">Log On Alarm</span><span class="txt"><asp:Label ID="lblLogOnAlarm" runat="server" value="val"></asp:Label></span></div>
        <div><span class="lbl">Log On Alarm Time</span><span class="txt"><asp:Label ID="lblLogOnAlarmTime" runat="server" value="val"></asp:Label></span></div>
        <div><span class="lbl">Log On Alarm Cleared</span><span class="txt"><asp:Label ID="lblLogOnAlarmCleared" runat="server" value="val"></asp:Label></span></div>
        <div><span class="lbl">Log On Alarm Excused</span><span class="txt"><asp:Label ID="lblLogOnAlarmExcused" runat="server" value="val"></asp:Label></span></div>
        <div><span class="lbl">Log On Alarm Comments</span><span class="txt"><asp:Label ID="lblLogOnAlarmComments" runat="server" value="val"></asp:Label></span></div>
        <div><span class="lbl">Roll Out Alarm</span><span class="txt"><asp:Label ID="lblRollOutAlarm" runat="server" value="val"></asp:Label></span></div>
        <div><span class="lbl">Roll Out Alarm Time</span><span class="txt"><asp:Label ID="lblRollOutAlarmTime" runat="server" value="val"></asp:Label></span></div>
        <div><span class="lbl">Roll Out Alarm Cleared</span><span class="txt"><asp:Label ID="lblRollOutAlarmCleared" runat="server" value="val"></asp:Label></span></div>
        <div><span class="lbl">Roll Out Alarm Excused</span><span class="txt"><asp:Label ID="lblRollOutAlarmExcused" runat="server" value="val"></asp:Label></span></div>
        <div><span class="lbl">Roll Out Alarm Comments</span><span class="txt"><asp:Label ID="lblRollOutAlarmComments" runat="server" value="val"></asp:Label></span></div>
        <div><span class="lbl">On Patrol Alarm</span><span class="txt"><asp:Label ID="lblOnPatrolAlarm" runat="server" value="val"></asp:Label></span></div>
        <div><span class="lbl">On Patrol Alarm Time</span><span class="txt"><asp:Label ID="lblOnPatrolAlarmTime" runat="server" value="val"></asp:Label></span></div>
        <div><span class="lbl">On Patrol Alarm Cleared</span><span class="txt"><asp:Label ID="lblOnPatrolAlarmCleared" runat="server" value="val"></asp:Label></span></div>
        <div><span class="lbl">On Patrol Alarm Excused</span><span class="txt"><asp:Label ID="lblOnPatrolAlarmExcused" runat="server" value="val"></asp:Label></span></div>
        <div><span class="lbl">On Patrol Alarm Comments</span><span class="txt"><asp:Label ID="lblOnPatrolAlarmComments" runat="server" value="val"></asp:Label></span></div>
        <div><span class="lbl">Roll In Alarm</span><span class="txt"><asp:Label ID="lblRollInAlarm" runat="server" value="val"></asp:Label></span></div>
        <div><span class="lbl">Roll In Alarm Time</span><span class="txt"><asp:Label ID="lblRollInAlarmTime" runat="server" value="val"></asp:Label></span></div>
        <div><span class="lbl">Roll In Alarm Cleared</span><span class="txt"><asp:Label ID="lblRollInAlarmCleared" runat="server" value="val"></asp:Label></span></div>
        <div><span class="lbl">Roll In Alarm Excused</span><span class="txt"><asp:Label ID="lblRollInAlarmExcused" runat="server" value="val"></asp:Label></span></div>
        <div><span class="lbl">Roll In Alarm Comments</span><span class="txt"><asp:Label ID="lblRollInAlarmComments" runat="server" value="val"></asp:Label></span></div>
        <div><span class="lbl">Log Off Alarm</span><span class="txt"><asp:Label ID="lblLogOffAlarm" runat="server" value="val"></asp:Label></span></div>
        <div><span class="lbl">Log Off Alarm Time</span><span class="txt"><asp:Label ID="lblLogOffAlarmTime" runat="server" value="val"></asp:Label></span></div>
        <div><span class="lbl">Log Off Alarm Cleared</span><span class="txt"><asp:Label ID="lblLogOffAlarmCleared" runat="server" value="val"></asp:Label></span></div>
        <div><span class="lbl">Log Off Alarm Excused</span><span class="txt"><asp:Label ID="lblLogOffAlarmExcused" runat="server" value="val"></asp:Label></span></div>
        <div><span class="lbl">Log Off Alarm Comments</span><span class="txt"><asp:Label ID="lblLogOffAlarmComments" runat="server" value="val"></asp:Label></span></div>
        <div><span class="lbl">Incident Alarm</span><span class="txt"><asp:Label ID="lblIncidentAlarm" runat="server" value="val"></asp:Label></span></div>
        <div><span class="lbl">Incident Alarm Time</span><span class="txt"><asp:Label ID="lblIncidentAlarmTime" runat="server" value="val"></asp:Label></span></div>
        <div><span class="lbl">Incident Alarm Cleared</span><span class="txt"><asp:Label ID="lblIncidentAlarmCleared" runat="server" value="val"></asp:Label></span></div>
        <div><span class="lbl">Incident Alarm Excused</span><span class="txt"><asp:Label ID="lblIncidentAlarmExcused" runat="server" value="val"></asp:Label></span></div>
        <div><span class="lbl">Incident Alarm Comments</span><span class="txt"><asp:Label ID="lblIncidentAlarmComments" runat="server" value="val"></asp:Label></span></div>
        <div><span class="lbl">GPS Issue Alarm</span><span class="txt"><asp:Label ID="lblGPSIssueAlarm" runat="server" value="val"></asp:Label></span></div>
        <div><span class="lbl">GPS Issue Start</span><span class="txt"><asp:Label ID="lblGPSIssueStart" runat="server" value="val"></asp:Label></span></div>
        <div><span class="lbl">GPS Issue Cleared</span><span class="txt"><asp:Label ID="lblGPSIssueCleared" runat="server" value="val"></asp:Label></span></div>
        <div><span class="lbl">GPS Issue Excused</span><span class="txt"><asp:Label ID="lblGPSIssueExcused" runat="server" value="val"></asp:Label></span></div>
        <div><span class="lbl">GPS Issue Comments</span><span class="txt"><asp:Label ID="lblGPSIssueComments" runat="server" value="val"></asp:Label></span></div>
        <div><span class="lbl">Stationary Alarm</span><span class="txt"><asp:Label ID="lblStationaryAlarm" runat="server" value="val"></asp:Label></span></div>
        <div><span class="lbl">Stationary Start</span><span class="txt"><asp:Label ID="lblStationaryStart" runat="server" value="val"></asp:Label></span></div>
        <div><span class="lbl">Stationary Cleared</span><span class="txt"><asp:Label ID="lblStationaryCleared" runat="server" value="val"></asp:Label></span></div>
        <div><span class="lbl">Stationary Excused</span><span class="txt"><asp:Label ID="lblStationaryExcused" runat="server" value="val"></asp:Label></span></div>
        <div><span class="lbl">Stationary Comments</span><span class="txt"><asp:Label ID="lblStationaryComments" runat="server" value="val"></asp:Label></span></div>
    </div>
    <asp:Timer ID="Timer1" runat="server" Interval="10000" ontick="Timer1_Tick">
    </asp:Timer>
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    </form>
</body>
</html>
