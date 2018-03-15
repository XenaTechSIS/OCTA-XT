<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CurrentIncidents.aspx.cs" Inherits="FPSService.Admin.CurrentIncidents" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Current Incidents and Assists</title>
    <link href="../css/SiteStyle.css" rel="stylesheet" type="text/css" />
    <link href="../css/HoverTable.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/jquery-1.8.1.min.js" type="text/javascript"></script>
    <script src="../Scripts/CurrentIncidentsToTable.js" type="text/javascript"></script>
    <script src="../Scripts/CurrentAssistsToTable.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="Title"><h1>FTT Dashboard - Current Incidents and Assists</h1></div>
    <a href="Dashboard.aspx">Back to Dashboard</a>
    <div id="CurrentIncidents">
    
    </div>
    <div id="CurrentAssists">
    
    </div>
    </form>
</body>
</html>
