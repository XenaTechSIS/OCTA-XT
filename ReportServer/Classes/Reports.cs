using System.Collections.Generic;


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
            var mySQL = new SQLCode();
            theseReports = mySQL.GetAllReports();
        }

        public static ReportData GetReportDataByName(string ReportName)
        {
            var mySQL = new SQLCode();
            var thisReport = mySQL.getReportDataByName(ReportName);
            return thisReport;
        }
    }


}