using System;
using System.IO;
using System.Web;

namespace FSP.Common
{
    public class FSPLogging
    {
        public static void WriteToLogFile(string _value)
        {
            var sLogFilePath = string.Empty;

            try
            {
                //check in web.config file is loggin is enabled

                sLogFilePath = HttpContext.Current.Server.MapPath("Log.txt");
                StreamWriter sw = null;
                if (!File.Exists(sLogFilePath))
                    sw = File.CreateText(sLogFilePath);
                else
                    sw = File.AppendText(sLogFilePath);

                sw.WriteLine(DateTime.Now + " " + _value);
                sw.Close();
            }
            catch
            {
            }
        }
    }
}