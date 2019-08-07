<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportMain.aspx.cs" Inherits="ReportServer.Reports.ReportMain" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>FSP Report Server</title>
    <script src="../Scripts/bootstrap.js" type="text/javascript"></script>
    <script src="../Scripts/jquery-ui-1.10.0.custom.js" type="text/javascript"></script>
    <script src="../Scripts/jquery-1.9.1.js" type="text/javascript"></script>
    <link href="../Styles/bootstrap-responsive.css" rel="stylesheet" type="text/css" />
    <link href="../Styles/jquery-ui-1.10.0.custom.css" rel="stylesheet" type="text/css" />
    <link href="../Styles/bootstrap.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <img src="../img/octa_survey%20layout_header.png" />
    <legend>Home</legend>
    <form id="form1" runat="server" class="form-horizontal">
        <div class="control-group">
            <asp:Label ID="Label1" runat="server" Text="Select Your Report" class="control-label"></asp:Label>
            <div class="controls">
                <asp:DropDownList ID="ddlReports" runat="server" CssClass="form-control" Style="margin-bottom: -1px">
                    <asp:ListItem>Alarms</asp:ListItem>
                    <asp:ListItem>Assists</asp:ListItem>
                    <asp:ListItem>Combined Driver CHP Comment Report</asp:ListItem>
                    <asp:ListItem>Driver Breaks</asp:ListItem>
                    <asp:ListItem>Driver and Beat Hours</asp:ListItem>
                    <asp:ListItem>Driver Alarm Comments</asp:ListItem>
                    <asp:ListItem>GPS Position Report</asp:ListItem>
                    <asp:ListItem>Schedules Report</asp:ListItem>
                    <asp:ListItem>Speeding Report</asp:ListItem>
                    <asp:ListItem>Truck Messages Report</asp:ListItem>
                    <asp:ListItem>Tip Report</asp:ListItem>
                    <asp:ListItem>Overtime Report</asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        <div class="control-group">
            <div class="controls">
                <asp:Button ID="btnGo" runat="server" OnClick="btnGo_Click" Text="Go" CssClass="btn btn-primary" />
            </div>
        </div>
    </form>
</body>
</html>
