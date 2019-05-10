<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Logon.aspx.cs" Inherits="ReportServer.Reports.Logon" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>OCTA Report Server Logon</title>
    <link href="../Styles/bootstrap.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <img src="../img/octa_survey%20layout_header.png" alt="octaheader" />
    <legend>OCTA Report Server Logon</legend>
    <form id="form1" runat="server" class="form-horizontal">
        <div class="control-group">
            <asp:Label ID="Label1" runat="server" Text="User Name" class="control-label"></asp:Label>
            <div class="controls">
                <asp:TextBox ID="txtUserName" runat="server" />
            </div>
        </div>
        <div class="control-group">
            <asp:Label ID="Label2" runat="server" Text="Password" class="control-label"></asp:Label>
            <div class="controls">
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" />
            </div>
        </div>
        <div class="control-group">
            <div class="controls">
                <asp:Button ID="btnGo" runat="server" Text="Logon" OnClick="btnGo_Click" CssClass="btn btn-primary" />
            </div>
        </div>
        <div>
            <asp:Panel ID="errPanel" runat="server" />
        </div>
    </form>
</body>
</html>
