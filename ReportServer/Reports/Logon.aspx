<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Logon.aspx.cs" Inherits="ReportServer.Reports.Logon" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>OCTA Report Server Logon</title>
    <link href="../Styles/bootstrap.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Panel ID="errPanel" runat="server" />
        <img src="../img/octa_survey%20layout_header.png" />
        </div>
        <div><h1>OCTA Report Server Logon</h1></div>
        <div>
            <div>User Name</div>
            <div><asp:TextBox ID="txtUserName" runat="server" /></div>
            <div>Password</div>
            <div><asp:TextBox ID="txtPassword" runat="server" TextMode="Password" /></div>
            <div><asp:Button ID="btnGo" runat="server" Text="Logon" CssClass="myButton" OnClick="btnGo_Click" /></div>
        </div>
    </form>
</body>
</html>
