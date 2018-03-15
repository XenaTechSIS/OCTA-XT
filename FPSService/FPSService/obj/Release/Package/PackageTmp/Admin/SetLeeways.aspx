<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SetLeeways.aspx.cs" Inherits="FPSService.Admin.SetLeeways" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Reset Leeway Times</title>
    <link href="../css/SiteStyle.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div id="Title"><h1>Reset Leeways</h1></div>
    
    <asp:HyperLink ID="HyperLink1" runat="server" 
        NavigateUrl="~/Admin/Dashboard.aspx">Dashboard</asp:HyperLink>
    &nbsp;<div id="leewayDiv" class="leeways">
        <div><span class="lbl">Extended</span><span class="txt"><asp:TextBox ID="txtExtended" runat=server></asp:TextBox></span></div>
        <div><span class="lbl">Log Off</span><span class="txt"><asp:TextBox ID="txtLogOff" runat=server></asp:TextBox></span></div>
        <div><span class="lbl">Log On</span><span class="txt"><asp:TextBox ID="txtLogOn" runat=server></asp:TextBox></span></div>
        <div><span class="lbl">GPS Issue</span><span class="txt"><asp:TextBox ID="txtGPSIssue" runat=server></asp:TextBox></span></div>
        <div><span class="lbl">Off Beat</span><span class="txt"><asp:TextBox ID="txtOffBeat" runat=server></asp:TextBox></span></div>
        <div><span class="lbl">On Patrol</span><span class="txt"><asp:TextBox ID="txtOnPatrol" runat=server></asp:TextBox></span></div>
        <div><span class="lbl">Roll In</span><span class="txt"><asp:TextBox ID="txtRollIn" runat=server></asp:TextBox></span></div>
        <div><span class="lbl">Roll Out</span><span class="txt"><asp:TextBox ID="txtRollOut" runat=server></asp:TextBox></span></div>
        <div><span class="lbl">Speeding</span><span class="txt"><asp:TextBox ID="txtSpeeding" runat=server></asp:TextBox></span></div>
        <div><span class="lbl">Stationary</span><span class="txt"><asp:TextBox ID="txtStationary" runat=server></asp:TextBox></span></div>
        <div><span class="lbl">Force Log Off</span><span class="txt"><asp:TextBox ID="txtForceLogoff" runat="server"></asp:TextBox></span></div>
        <div><asp:Button ID="btnUpdateVars" runat="server" Text="Update Vars" 
                onclick="btnUpdateVars_Click" /></div>
    </div>
    </form>
    </body>
</html>
