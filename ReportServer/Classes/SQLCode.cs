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
            ReportData thisReport = new ReportData();

            string ConnStr = SQLConn.GetConnString("PROD");
            string SQL = "SELECT ReportName, ConnString, SQL, cmdType, parameters FROM Reports WHERE ReportName = '" + ReportName + "'";
            using (SqlConnection conn = new SqlConnection(ConnStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(SQL, conn);
                SqlDataReader rdr = cmd.ExecuteReader();
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
            List<ReportData> theseReports = new List<ReportData>();
            string ConnStr = SQLConn.GetConnString("PROD");
            string SQL = "SELECT ReportName, ConnString, SQL, cmdType, parameters FROM Reports";
            using (SqlConnection conn = new SqlConnection(ConnStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(SQL, conn);
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ReportData thisReport = new ReportData
                    {
                        ReportName = rdr["ReportName"].ToString(),
                        ConnString = rdr["ConnString"].ToString(),
                        SQL = rdr["SQL"].ToString(),
                        cmdType = rdr["cmdType"].ToString()
                    };
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