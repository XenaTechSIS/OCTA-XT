﻿using System;
using System.Web.UI;

namespace ReportServer.Reports
{
    public partial class Logon : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnGo_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUserName.Text))
            {
                errPanel.Controls.Add(new LiteralControl("<h2 style=\"color:red;background-color:yellow\">Missing User Name</h2>"));
                return;
            }
            if (string.IsNullOrEmpty(txtPassword.Text))
            {
                errPanel.Controls.Add(new LiteralControl("<h2 style=\"color:red;background-color:yellow\">Missing Password</h2>"));
                return;
            }
            if (txtUserName.Text.ToUpper() != "OCTAADMIN" || txtPassword.Text != "OCTA2013")
            {
                errPanel.Controls.Add(new LiteralControl("<h2 style=\"color:red;background-color:yellow\">Incorrect User Name or Password</h2>"));
                return;
            }
            Session["UserOK"] = "OK";
            Response.Redirect("ReportMain.aspx");
        }
    }
}