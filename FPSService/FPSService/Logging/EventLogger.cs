using System;
using System.Diagnostics;

namespace FPSService.Logging
{
    public class EventLogger
    {
        private void CheckLogStatus()
        {
            try
            {
                if (!EventLog.SourceExists("FSPService")) EventLog.CreateEventSource("FSPService", "FSPServiceLog");
            }
            catch
            {
            }
        }

        public void LogEvent(string eventData, bool error)
        {
            try
            {
                CheckLogStatus();
                var eventLog1 = new EventLog {Source = "FSPService", Log = "FSPServiceLog"};
                if (error)
                    eventLog1.WriteEntry(eventData, EventLogEntryType.Error);
                else
                    eventLog1.WriteEntry(eventData);
            }
            catch
            {
            }
        }
    }
}