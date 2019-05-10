<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CombinedReport.aspx.cs" Inherits="ReportServer.Reports.CombinedReport" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Combined Comments Report</title>
    <script src="../Scripts/jquery-1.9.1.js" type="text/javascript"></script>
    <script src="../Scripts/jquery-ui-1.10.0.custom.js" type="text/javascript"></script>
    <script src="../Scripts/bootstrap.js" type="text/javascript"></script>
    <link href="../Styles/jquery-ui-1.10.0.custom.css" rel="stylesheet" type="text/css" />
    <link href="../Styles/bootstrap.css" rel="stylesheet" type="text/css" />
    <link href="../Styles/bootstrap-responsive.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        $(document).ready(function () {
            $('#startDT').datepicker();
            $('#endDT').datepicker();
        });

    </script>
</head>
<body>
    <img src="../img/octa_survey%20layout_header.png" alt="octaheader" />
    <div>
        <a href="ReportMain.aspx">Home</a>
    </div>

    <legend>Combined Comments Report</legend>

    <p><b><i>Note: This report only displays alerts that a driver can comment on: Late Logon, Late Roll Out, Late On Patrol, Early Login, and Early Log Off</i></b></p>

    <form id="form1" runat="server" class="form-horizontal">
        <div class="control-group">
            <asp:Label ID="Label1" runat="server" Text="Select Start Date" class="control-label"></asp:Label>
            <div class="controls">
                <input type="text" value="01/01/2013" id="startDT" runat="server" />
            </div>
        </div>
        <div class="control-group">
            <asp:Label ID="Label2" runat="server" Text="Select End Date" class="control-label"></asp:Label>
            <div class="controls">
                <input type="text" value="01/01/2013" id="endDT" runat="server" onclick="return endDT_onclick()" />
            </div>
        </div>
        <div class="control-group">
            <div class="controls">
                <asp:Button ID="btnGetReport" runat="server" Text="Get Report"
                    OnClick="btnGetReport_Click" CssClass="btn btn-primary" />
                <asp:Button ID="btnExportExcel" runat="server"
                    Text="Export Excel (Wait for data to load)" OnClick="btnExportExcel_Click" CssClass="btn btn-primary" />
            </div>
        </div>

        <div id="tableDiv">

            <asp:GridView ID="gvData" runat="server" CellPadding="4" ForeColor="#333333"
                GridLines="None">
                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                <EditRowStyle BackColor="#999999" />
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="Black" />
                <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                <SortedAscendingCellStyle BackColor="#E9E7E2" />
                <SortedAscendingHeaderStyle BackColor="#506C8C" />
                <SortedDescendingCellStyle BackColor="#FFFDF8" />
                <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
            </asp:GridView>

        </div>
    </form>
</body>
</html>
