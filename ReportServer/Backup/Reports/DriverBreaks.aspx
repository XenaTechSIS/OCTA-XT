<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DriverBreaks.aspx.cs" Inherits="ReportServer.Reports.DriverBreaks" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>FSP Driver Breaks</title>
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
    <form id="form1" runat="server">
    <img src="../img/octa_survey%20layout_header.png" />
    <header>Driver Break and Lunch Report</header>
    <div>
        <asp:Label ID="Label1" runat="server" Text="Select Start Date"></asp:Label>
        <input type="text" value="01/01/2013" id="startDT" runat="server"/>
    </div>
    <div>
        <asp:Label ID="Label2" runat="server" Text="Select End Date"></asp:Label>
        <input type="text" value="01/01/2013" id="endDT" runat="server" onclick="return endDT_onclick()" />
    </div>
    <div>
        <asp:Label ID="lblSelectContractors" Text="Select Contract Company" runat="server"></asp:Label>
        <asp:DropDownList ID="ddlContractors" runat="server">
        </asp:DropDownList>
        <asp:Button ID="btnGetDrivers" runat="server" Text="Load Drivers" 
            onclick="btnGetDrivers_Click" />
    </div>
    <div>
        <asp:Label ID="Label3" runat="server" Text="Select Drivers"></asp:Label>
        <asp:DropDownList ID="ddlDrivers" runat="server">
        </asp:DropDownList>
    &nbsp;
        </div>
    <div>
        
        <asp:Button ID="btnGetReport" runat="server" Text="Get Report" 
            onclick="btnGetReport_Click" />
        
        <asp:Button ID="btnExportExcel" runat="server" onclick="btnExportExcel_Click" 
            Text="Export Excel (Wait for data to load)" />
        
    </div>
        <div>
    <a href="ReportMain.aspx">Home</a>
    <hr />
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
