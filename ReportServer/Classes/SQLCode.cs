using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace ReportServer.Classes
{
    public class SQLCode
    {
        public ReportData getReportDataByName(string ReportName)
        {
            var thisReport = new ReportData();

            var connStr = SQLConn.GetConnString("PROD");
            var sql = "SELECT ReportName, ConnString, SQL, cmdType, parameters FROM Reports WHERE ReportName = '" + ReportName + "'";
            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                var cmd = new SqlCommand(sql, conn);
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    thisReport.ReportName = rdr["ReportName"].ToString();
                    thisReport.ConnString = rdr["ConnString"].ToString();
                    thisReport.SQL = rdr["SQL"].ToString();
                    thisReport.cmdType = rdr["cmdType"].ToString();
                    if (rdr["parameters"] != DBNull.Value)
                    {
                        ArrayList Params = new ArrayList();
                        string[] parmList = rdr["parameters"].ToString().Split('|');
                        for (int i = 0; i < parmList.Count(); i++)
                        {
                            Params.Add(parmList[i].ToString());
                        }
                        thisReport.parameters = Params;
                    }
                }
                rdr.Close();
                rdr = null;
                cmd = null;
                conn.Close();
            }
            return thisReport;
        }

        public List<ReportData> GetAllReports()
        {
            var theseReports = new List<ReportData>();
            var connStr = SQLConn.GetConnString("PROD");
            var sql = "SELECT ReportName, ConnString, SQL, cmdType, parameters FROM Reports";
            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                var cmd = new SqlCommand(sql, conn);
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var thisReport = new ReportData
                    {
                        ReportName = rdr["ReportName"].ToString(),
                        ConnString = rdr["ConnString"].ToString(),
                        SQL = rdr["SQL"].ToString(),
                        cmdType = rdr["cmdType"].ToString()
                    };
                    if (rdr["parameters"] != DBNull.Value)
                    {
                        var Params = new ArrayList();
                        var parmList = rdr["parameters"].ToString().Split('|');
                        for (int i = 0; i < parmList.Count(); i++)
                        {
                            Params.Add(parmList[i].ToString());
                        }
                        thisReport.parameters = Params;
                    }
                    theseReports.Add(thisReport);
                }
                rdr.Close();
                rdr = null;
                cmd = null;
                conn.Close();
            }

            return theseReports;
        }
    }
}