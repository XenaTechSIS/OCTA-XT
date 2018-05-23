using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Security;
using FSP.Domain.Model;
using FSP.Web.Models;
using FSP.Web.TowTruckServiceRef;

namespace FSP.Web.Helpers
{
    public static class Util
    {
        public static void CreateAssist(UIIncidentDispatch incidentDispatch)
        {
            using (var dc = new FSPDataContext())
            {
                using (var service = new TowTruckServiceClient())
                {
                    var thisIncident = new IncidentIn();

                    var incidentId = Guid.NewGuid();
                    var timeStamp = DateTime.Now;
                    var createdBy = MembershipExtensions.GetUserId();

                    thisIncident.IncidentID = incidentId;
                    thisIncident.FreewayID = Convert.ToInt32(incidentDispatch.FreewayId);

                    var loc = dc.Locations.FirstOrDefault(p => p.LocationCode == incidentDispatch.LocationName);
                    if (loc != null)
                        thisIncident.LocationID = loc.LocationID;

                    thisIncident.TimeStamp = timeStamp;
                    thisIncident.CreatedBy = createdBy;
                    thisIncident.Description = incidentDispatch.Description;
                    thisIncident.IncidentNumber = string.Empty;

                    //create incident
                    service.AddIncident(thisIncident);

                    foreach (var truck in incidentDispatch.SelectedTrucks)
                    {
                        var thisAssist = new AssistReq
                        {
                            AssistID = Guid.NewGuid(),
                            IncidentID = incidentId
                        };


                        var dbTruck = dc.FleetVehicles.FirstOrDefault(p => p.VehicleNumber == truck);

                        if (dbTruck != null)
                        {
                            thisAssist.FleetVehicleID = dbTruck.FleetVehicleID;
                            thisAssist.ContractorID = dbTruck.ContractorID;
                        }                        

                        thisAssist.DispatchTime = DateTime.Now;
                        thisAssist.x1097 = DateTime.Now;
                        service.AddAssist(thisAssist);
                    }
                }
            }
        }

        #region Status Codes

        public static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return
                        "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return
                        "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return
                        "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return
                        "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }

        #endregion

        public static void SendEmail(string subject, string body)
        {
            var toEmailAddress = ConfigurationManager.AppSettings["OCTAAdminEmailAddress"];
            var toName = ConfigurationManager.AppSettings["OCTAAdminName"];

            var fromAddress = new MailAddress("latatrax@gmail.com", "LATA Trax");
            var toAddress = new MailAddress(toEmailAddress, toName);
            const string fromPassword = "L@T@2013";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                IsBodyHtml = true,
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }

        public static void WriteToEventLog(string message)
        {
            try
            {
                const string sSource = "FSPWeb";
                const string sLog = "Application";

                if (!EventLog.SourceExists(sSource))
                    EventLog.CreateEventSource(sSource, sLog);

                EventLog.WriteEntry(sSource, message, EventLogEntryType.Error);
            }
            catch (Exception ex)
            {
                WriteToLog(ex.Message, "Error.txt");
            }

            Debug.WriteLine(message);
        }

        public static void WriteToLog(string message, string logName)
        {
            Console.WriteLine(message);
            try
            {
                WriteToFile(message, logName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"ERROR WriteToFile: " + ex.Message);
            }

            //if (ConfigurationManager.AppSettings["IsLoggingEnabled"] == "true")
            //    try
            //    {
            //        var sLogFilePath = ConfigurationManager.AppSettings["LogFileLocation"] + logName;                    
            //        var sw = !File.Exists(sLogFilePath) ? File.CreateText(sLogFilePath) : File.AppendText(sLogFilePath);
            //        sw.WriteLine(DateTime.Now + " " + message);
            //        sw.Close();
            //    }
            //    catch
            //    {
            //        // ignored
            //    }

        }

        private static readonly ReaderWriterLock Locker = new ReaderWriterLock();
        public static void WriteToFile(string input, string fileName)
        {
            try
            {
                if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(fileName)) return;
                Locker.AcquireWriterLock(int.MaxValue);
                using (var w = File.AppendText(fileName))
                {
                    Debug.WriteLine(input);
                    w.WriteLine(input);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                Locker.ReleaseWriterLock();
            }
        }

        private static string _logsPath;
        public static string LogsPath
        {
            get
            {
                if (!string.IsNullOrEmpty(_logsPath)) return _logsPath;
                var binDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                if (binDirectory == null) return _logsPath;
                _logsPath = Path.Combine(binDirectory, @"..\");

                var webServerExecutionPath = HttpContext.Current?.Server.MapPath("~");
                if (!string.IsNullOrEmpty(webServerExecutionPath))
                    _logsPath = webServerExecutionPath;

                return _logsPath;
            }
            set { _logsPath = value; }
        }

        public static void LogInfo(string message)
        {
            var messageEntry = $"{DateTime.UtcNow} {message}";
            var logFilePath = LogsPath + "\\Logs.txt";
            WriteToLog(messageEntry, logFilePath);
        }

        public static void LogError(string message)
        {
            var messageEntry = $"{DateTime.UtcNow} {message}";
            var logFilePath = LogsPath + "\\Errors.txt";
            WriteToLog(messageEntry, logFilePath);
        }
    }
}