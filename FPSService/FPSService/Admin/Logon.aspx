<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Logon.aspx.cs" Inherits="FPSService.Admin.Logon" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../css/SiteStyle.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <h1>Please log on to continue</h1>
    </div>
    <div>
        <label class="logonText">User Name</label><span class="logonBox"><asp:TextBox 
            ID="txtUserName" runat="server"></asp:TextBox></span>
        
    </div>
        <div>
        <label class="logonText">Password</label><span class="logonBox"><asp:TextBox 
                ID="txtPassword" runat="server" TextMode="Password"></asp:TextBox></span>
            
    </div>
    <div>
        <asp:Button ID="Button1" runat="server" Text="Log On" onclick="Button1_Click" />
    </div>
    </form>
</body>
</html>
