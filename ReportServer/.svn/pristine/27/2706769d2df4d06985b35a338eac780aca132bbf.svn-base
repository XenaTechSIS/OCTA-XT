using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace ReportServer.Classes
{
    public static class SQLConn
    {
        public static string GetConnString(string Type)
        {
            if (Type == "PROD")
            {
                return ConfigurationManager.AppSettings["PROD"].ToString();
            }
            else
            {
                return ConfigurationManager.AppSettings["ARCHIVE"].ToString();
            }
        }
    }
}