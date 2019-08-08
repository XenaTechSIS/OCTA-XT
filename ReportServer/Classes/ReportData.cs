using System.Collections;

namespace ReportServer.Classes
{
    public class ReportData
    {
        public string ReportName { get; set; }
        public string ConnString { get; set; }
        public string SQL { get; set; }
        public string cmdType { get; set; }
        public ArrayList parameters { get; set; }
    }
}