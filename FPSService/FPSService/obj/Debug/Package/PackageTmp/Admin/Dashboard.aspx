<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="FPSService.Admin.Dashboard" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="../css/SiteStyle.css" rel="stylesheet" type="text/css" />
    <link href="../css/HoverTable.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/jquery-1.8.1.min.js" type="text/javascript"></script>
    <script src="../Scripts/CurrentConnectsJSONtoTable.js" type="text/javascript"></script>
    <!--<script src="../Scripts/CurrentStateJSONtoTable.js" type="text/javascript"></script>-->
    <title>FTT Dashboard</title>
</head>
<body>
    <form id="form1" runat="server">
       
       <div id="Title"><h1>FTT Dashboard - Provided by LATA</h1></div>
       <div><a href="CurrentIncidents.aspx">Current Incidents and Assists </a>&nbsp; <a href="LogOffDriver.aspx">Log Off Driver</a>
       &nbsp VarName &nbsp
           <asp:TextBox ID="txtVarName" runat="server"></asp:TextBox>
&nbsp; VarValue
           <asp:TextBox ID="txtVarValue" runat="server" style="width: 128px"></asp:TextBox>
           <asp:Button ID="btnSetVar" runat="server" onclick="btnSetVar_Click" Text="Set" />
       &nbsp;
           &nbsp;
           <asp:HyperLink ID="HyperLink1" runat="server" 
               NavigateUrl="~/Admin/CurrentBeats.aspx">View Beats</asp:HyperLink>
       &nbsp;<asp:HyperLink ID="HyperLink2" runat="server" 
               NavigateUrl="~/Admin/Yards.aspx">View Yards</asp:HyperLink>
       &nbsp;<asp:HyperLink ID="HyperLink3" runat="server" 
               NavigateUrl="~/Admin/CurrentBeatSchedules.aspx">View Beat Schedules</asp:HyperLink>
       &nbsp;<asp:HyperLink ID="HyperLink4" runat="server" 
               NavigateUrl="~/Admin/SetLeeways.aspx">View Leeways</asp:HyperLink>
       &nbsp;<asp:HyperLink ID="HyperLink5" runat="server" 
               NavigateUrl="~/Admin/TruckMessages.aspx">Msgs</asp:HyperLink>
       </div>
        <div id="msgHead" class="subTitle"><h2>Send Message to FTT</h2></div>

        <div class="colmask threecol">
	        <div class="colmid">
		        <div class="colleft">
			    <div class="col1"> <!--Middle COlumn-->
                    <h3>Select Message </h3>
                    <asp:DropDownList ID="ddlMessages" runat="server" Width="250px">
                        <asp:ListItem>GetGPS</asp:ListItem>
                        <asp:ListItem>GetState</asp:ListItem>
                        <asp:ListItem>Reboot</asp:ListItem>
                        <asp:ListItem>GetVar</asp:ListItem>
                        <asp:ListItem>SetVar</asp:ListItem>
                    </asp:DropDownList>
                </div><!-- Middle COlumn-->
                <div class="col2"> <!--Left COlumn-->
                    <h3>Select FTT</h3>
                    <asp:DropDownList ID="ddlFTTs" runat="server" Height="22px" Width="250px">
                    </asp:DropDownList>
                </div><!--Left Column-->
                <div class="col3"> <!--Right Column-->
                    <h3>Option: Set Value (Name|Value)</h3>
                    <asp:TextBox ID="txtMessageVal" runat="server" Width="250px" 
                        ToolTip=" For GetVar use Name|VarName, for SetVar use VarName|Value" />
                </div><!--Right Column-->
            </div> <!--CoMid-->
        </div> <!--ColMask-->

        
        <div id="selMessage">
            
        </div>
        <div id="extendMessage">

        </div>
        <div id="btn">
            <asp:Button ID="Button1" runat="server" Text="Send Message" 
                onclick="Button1_Click" ToolTip="Send selected message to selected FTT" />
            <asp:Button ID="Button2" runat="server" Text="Refresh FTT List" Width="146px" 
                ToolTip="Refresh the Select FTT List" onclick="Button2_Click" />
            <asp:Button ID="Button3" runat="server" onclick="Button3_Click" 
                Text="Reset Selected FTT" Width="138px" />
        </div>
    </div>
    <hr />
    <div id="connectCount"><label>Connection Count:&nbsp;</label><label id="conCount"></label></div>
    <div id="CurrentConnects">
    
    </div>
    <!--
    <div id="CurrentState">
    
    </div>-->
    </form>
</body>
</html>
