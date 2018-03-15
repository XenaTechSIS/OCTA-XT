using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Timers;
using System.Collections;

namespace FPSService.DataClasses
{
    public class BulkLogger
    {
        public BulkLogger()
        {
            Timer tmrLogTrucks = new Timer(5000);
            tmrLogTrucks.Elapsed += new ElapsedEventHandler(tmrLogTrucks_Elapsed);
            tmrLogTrucks.Enabled = true;
        }

        void tmrLogTrucks_Elapsed(object sender, ElapsedEventArgs e)
        {
            SQL.SQLCode mySQL = new SQL.SQLCode();
            foreach (TowTruck.TowTruck thisTruck in DataClasses.GlobalData.currentTrucks.ToList())
            {
                bool Alarms = false;
                if (thisTruck.Status.OutOfBoundsAlarm == true || thisTruck.Status.SpeedingAlarm == true)
                {
                    Alarms = true;
                }
                ArrayList arrParams = new ArrayList();
                arrParams.Add("@Direction^" + thisTruck.GPSPosition.Head);
                arrParams.Add("@VehicleStatus^" + thisTruck.Status.VehicleStatus);
                arrParams.Add("@LastUpdate^" + thisTruck.GPSPosition.Time.ToString());
                arrParams.Add("@BeatSegmentID^" + thisTruck.GPSPosition.BeatSegmentID.ToString());
                arrParams.Add("@VehicleID^" + thisTruck.TruckNumber);
                arrParams.Add("@Speed^" + thisTruck.GPSPosition.Speed.ToString());
                arrParams.Add("@Alarms^" + Alarms);
                arrParams.Add("@DriverID^" + thisTruck.Driver.DriverID.ToString());
                arrParams.Add("@VehicleNumber^" + thisTruck.Identifier.ToString());
                arrParams.Add("@BeatID^" + thisTruck.GPSPosition.BeatID);
                arrParams.Add("@Position^" + thisTruck.GPSPosition.Position);
                arrParams.Add("@SpeedingAlarm^" + thisTruck.Status.SpeedingAlarm);
                arrParams.Add("@SpeedingValue^" + thisTruck.Status.SpeedingValue);
                if (thisTruck.Status.SpeedingLocation != null)
                {
                    arrParams.Add("@SpeedingLocation^" + thisTruck.Status.SpeedingLocation);
                }
                else
                {
                    arrParams.Add("@SpeedingLocation^" + thisTruck.GPSPosition.Position);
                }
                arrParams.Add("@SpeedingTime^" + thisTruck.Status.SpeedingTime.ToString());
                arrParams.Add("@OutOfBoundsAlarm^" + thisTruck.Status.OutOfBoundsAlarm);
                arrParams.Add("@OutOfBoundsMessage^" + thisTruck.Status.OutOfBoundsMessage);
                arrParams.Add("@AssignedBeat^" + thisTruck.assignedBeat.BeatID);
                mySQL.LogGPS("SetGPS", arrParams);
            }
        }
    }
}