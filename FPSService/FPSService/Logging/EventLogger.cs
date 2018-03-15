using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;

namespace FPSService.Logging
{
    public class EventLogger
    {
        private void CheckLogStatus()
        {
            if (!EventLog.SourceExists("FSPService"))
            {
                EventLog.CreateEventSource("FSPService", "FSPServiceLog");
            }
        }

        public void LogEvent(string EventData, bool error)
        {
            CheckLogStatus();
            EventLog eventLog1 = new EventLog();
            eventLog1.Source = "FSPService";
            eventLog1.Log = "FSPServiceLog";
            if (error == true)
            {
                eventLog1.WriteEntry(EventData, EventLogEntryType.Error);
            }
            else
            {
                eventLog1.WriteEntry(EventData);
            }
        }
    }
}