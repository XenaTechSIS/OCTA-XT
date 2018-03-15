using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace ReportServer.Classes
{
    public static class Reports
    {
        //Don't know if I'm going to use this
        public static List<ReportData> theseReports = new List<ReportData>();

        //Don't know if I'm going to use this
        public static void GetReports()
        {
            theseReports.Clear();
            SQLCode mySQL = new SQLCode();
            theseReports = mySQL.GetAllReports();
            mySQL = null;
        }

        public static ReportData GetReportDataByName(string ReportName)
        {
            SQLCode mySQL = new SQLCode();
            ReportData thisReport = mySQL.getReportDataByName(ReportName);
            mySQL = null;
            return thisReport;
        }
    }

    
}