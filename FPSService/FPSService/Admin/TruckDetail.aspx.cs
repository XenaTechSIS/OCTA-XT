﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FPSService.Admin
{
    public partial class TruckDetail : System.Web.UI.Page
    {
        string IPAddress;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Logon"] == null)
            {
                Response.Redirect("Logon.aspx");
            }
            string logon = Session["Logon"].ToString();
            if (logon != "true")
            {
                Response.Redirect("Logon.aspx");
            }
            IPAddress = Request.QueryString["ip"];
            if (!string.IsNullOrEmpty(IPAddress))
            {
                TowTruck.TowTruck thisTruck = DataClasses.GlobalData.FindTowTruck(IPAddress);
                if (thisTruck != null)
                {
                    if (!string.IsNullOrEmpty(thisTruck.Driver.LastName))
                    {
                        lblDriverName.Text = thisTruck.Driver.LastName + ", " + thisTruck.Driver.FirstName;
                    }
                    else
                    { lblDriverName.Text = "unknown"; }
                    if (!string.IsNullOrEmpty(thisTruck.Driver.FSPID))
                    {
                        lblDriverFSPID.Text = thisTruck.Driver.FSPID;
                    }
                    else
                    { lblDriverFSPID.Text = "unknown"; }
                    if (!string.IsNullOrEmpty(thisTruck.assignedBeat.BeatNumber))
                    {
                        lblAssignedBeat.Text = thisTruck.assignedBeat.BeatNumber;
                    }
                    else
                    { lblAssignedBeat.Text = "unknown"; }
                    if (!string.IsNullOrEmpty(thisTruck.Extended.VehicleType))
                    {
                        lblVehicleType.Text = thisTruck.Extended.VehicleType;
                    }
                    else
                    { lblVehicleType.Text = "unknown"; }
                    if (thisTruck.Extended.VehicleYear != 0)
                    {
                        lblVehicleYear.Text = thisTruck.Extended.VehicleYear.ToString();
                    }
                    else
                    { lblVehicleYear.Text = "unknown"; }
                    if (!string.IsNullOrEmpty(thisTruck.Extended.VehicleModel))
                    {
                        lblVehicleModel.Text = thisTruck.Extended.VehicleModel;
                    }
                    else
                    { lblVehicleModel.Text = "unknown"; }
                    if (!string.IsNullOrEmpty(thisTruck.Extended.LicensePlate))
                    {
                        lblLicensePlate.Text = thisTruck.Extended.LicensePlate;
                    }
                    else
                    { lblLicensePlate.Text = "unknown"; }
                    if (thisTruck.Extended.RegistrationExpireDate != null)
                    {
                        lblRegistrationExpiration.Text = thisTruck.Extended.RegistrationExpireDate.ToString();
                    }
                    else
                    { lblRegistrationExpiration.Text = "unknown"; }
                    if (thisTruck.Extended.InsuranceExpireDate != null)
                    {
                        lblInsuranceExpiration.Text = thisTruck.Extended.InsuranceExpireDate.ToString();
                    }
                    else
                    { lblRegistrationExpiration.Text = "unknown"; }
                    if (thisTruck.Extended.LastCHPInspection != null)
                    {
                        lblLastCHPInspection.Text = thisTruck.Extended.LastCHPInspection.ToString();
                    }
                    else
                    { lblLastCHPInspection.Text = "unknown"; }
                    if (thisTruck.Extended.ProgramEndDate != null)
                    {
                        lblProgramEndDate.Text = thisTruck.Extended.ProgramEndDate.ToString();
                    }
                    else
                    { lblProgramEndDate.Text = "unknown"; }
                }
            }
        }
    }
}