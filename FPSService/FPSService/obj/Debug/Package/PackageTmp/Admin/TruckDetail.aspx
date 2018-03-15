<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TruckDetail.aspx.cs" Inherits="FPSService.Admin.TruckDetail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../css/HoverTable.css" rel="stylesheet" type="text/css" />
    <link href="../css/SiteStyle.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/jquery-1.8.1.min.js" type="text/javascript"></script>
    <script src="../Scripts/TruckDetail.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div id="Title"><h1>FTT Dashboard - Selected Truck Detail</h1></div>
        <a href="Dashboard.aspx">Back to Dashboard</a>
        <div class="multiColumn">
        <div class="leftCol">
        <table id="dataTable" class="hovertable">
            <tr><td><label class="lblDescriptor">Truck Number</label></td><td><label id="lblTruckNumber" class="lblVal">Truck Number</label></td></tr>
            <tr><td><label class="lblDescriptor">IP Address</label></td><td><label id="lblIPAddress" class="lblVal">IP Address</label></td></tr>
            <tr><td><label class="lblDescriptor">Direction</label></td><td><label id="lblDirection" class="lblVal">Direction</label></td></tr>
            <tr><td><label class="lblDescriptor">Speed</label></td><td><label id="lblSpeed" class="lblVal">Speed</label></td></tr>
            <tr><td><label class="lblDescriptor">Lat</label></td><td><label id="lblLat" class="lblVal">Lat</label></td></tr>
            <tr><td><label class="lblDescriptor">Lon</label></td><td><label id="lblLon" class="lblVal">Lon</label></td></tr>
            <tr><td><label class="lblDescriptor">Vehicle State</label></td><td><label id="lblVehicleState" class="lblVal">Vehicle State</label></td></tr>
            <tr><td><label class="lblDescriptor">Alarms</label></td><td><label id="lblAlarms" class="lblVal">Alarms</label></td></tr>
            <tr><td><label class="lblDescriptor">Speeding Alarm</label></td><td><label id="lblSpeedingAlarm" class="lblVal">Speeding Alarm</label></td></tr>
            <tr><td><label class="lblDescriptor">Speeding Value</label></td><td><label id="lblSpeedingValue" class="lblVal">Speeding Value</label></td></tr>
            <tr><td><label class="lblDescriptor">Speeding Time</label></td><td><label id="lblSpeedingTime" class="lblVal">Speeding Time</label></td></tr>
            <tr><td><label class="lblDescriptor">Out of Bounds Alarm</label></td><td><label id="lblOutOfBoundsAlarm" class="lblVal">Out of Bounds Alarm</label></td></tr>
            <tr><td><label class="lblDescriptor">Out of Bounds Message</label></td><td><label id="lblOutOfBoundsMessage" class="lblVal">Out of Bounds Message</label></td></tr>
            <tr><td><label class="lblDescriptor">Out of Bounds Time</label></td><td><label id="lblOutOfBoundsTime" class="lblVal">Out of Bounds Time</label></td></tr>
            <tr><td><label class="lblDescriptor">Heading</label></td><td><label id="lblHeading" class="lblVal">Heading</label></td></tr>
            <tr><td><label class="lblDescriptor">Last Message</label></td><td><label id="lblLastMessage" class="lblVal">Last Message</label></td></tr>
            <tr><td><label class="lblDescriptor">Contractor Name</label></td><td><label id="lblContractorName" class="lblVal">lblContractorName</label></td></tr>
            <tr><td><label class="lblDescriptor">Beat Number</label></td><td><label id="lblBeatNumber" class="lblVal">Beat Number</label></td></tr>
            <tr><td><label class="lblDescriptor">GPS Rate</label></td><td><label id="lblGPSRate" class="lblVal">GPS Rate</label></td></tr>
            <tr><td><label class="lblDescriptor">GPS Status</label></td><td><label id="lblGPSStatus" class="lblVal">GPS Status</label></td></tr>
            <tr><td><label class="lblDescriptor">GPS DOP</label></td><td><label id="lblGPSDOP" class="lblVal">GPS DOP</label></td></tr>
            <tr><td><label class="lblDescriptor">Log</label></td><td><label id="lblLog" class="lblVal">Log</label></td></tr>
            <tr><td><label class="lblDescriptor">Version</label></td><td><label id="lblVersion" class="lblVal">Version</label></td></tr>
            <tr><td><label class="lblDescriptor">Server IP</label></td><td><label id="lblServerIP" class="lblVal">Server IP</label></td></tr>
            <tr><td><label class="lblDescriptor">SFTP Server IP</label></td><td><label id="lblSFTPServerIP" class="lblVal">SFTP Server IP</label></td></tr>
        </table>
        </div>
        <div class="rightCol">
            <table id="extendedTable" class="hovertable">
                <tr><td>Driver Name</td><td><asp:Label ID="lblDriverName" runat="server" Text="Driver Name"></asp:Label></td></tr>
                <tr><td>Driver FSPID</td><td><asp:Label ID="lblDriverFSPID" runat="server" Text="Driver FSP ID"></asp:Label></td></tr>
                <tr><td>Assigned Beat</td><td><asp:Label ID="lblAssignedBeat" runat="server" Text="Assigned Beat"></asp:Label></td></tr>
                <tr><td>Vehicle Type</td><td><asp:Label ID="lblVehicleType" runat="server" Text="Vehicle Type"></asp:Label></td></tr>
                <tr><td>Vehicle Year</td><td><asp:Label ID="lblVehicleYear" runat="server" Text="Vehicle Year"></asp:Label></td></tr>
                <tr><td>Vehicle Make</td><td><asp:Label ID="lblVehicleMake" runat="server" Text="Vehicle Make"></asp:Label></td></tr>
                <tr><td>Vehicle Model</td><td><asp:Label ID="lblVehicleModel" runat="server" Text="Vehicle Model"></asp:Label></td></tr>
                <tr><td>License Plate</td><td><asp:Label ID="lblLicensePlate" runat="server" Text="License Plate"></asp:Label></td></tr>
                <tr><td>Registration Expiration</td><td><asp:Label ID="lblRegistrationExpiration" runat="server" Text="Registration Expiration"></asp:Label></td></tr>
                <tr><td>Insurance Expiration</td><td><asp:Label ID="lblInsuranceExpiration" runat="server" Text="Insurance Expiration"></asp:Label></td></tr>
                <tr><td>Last CHP Inspection</td><td><asp:Label ID="lblLastCHPInspection" runat="server" Text="Last CHP Inspection"></asp:Label></td></tr>
                <tr><td>Program End Date</td><td><asp:Label ID="lblProgramEndDate" runat="server" Text="Program End Date"></asp:Label></td></tr>
            </table>
        </div>
        <div class="clear"></div>
     </div>

    </form>
</body>
</html>
