using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Configuration;
using NLog;
using Newtonsoft.Json;

namespace WinLog
{
    class WinLogService : System.ServiceProcess.ServiceBase
    {

        public WinLogService()
        {
            this.AutoLog = false;
            this.CanHandlePowerEvent = true;
            this.CanPauseAndContinue = true;
            this.ServiceName = "WinLogd";
        }

        protected override void OnStart(string[] args)
        {
            
            Logger logger = LogManager.GetLogger("WinLog");
            logger.Info("WinLog is starting...");

            //Handle Event Log Events
            EventLog[] mlogs = EventLog.GetEventLogs();
            logger.Debug("Retrived {0} log(s) to register with EntryWrittenHandeler", mlogs.Length);

            foreach (EventLog log in mlogs)
            {
                logger.Debug("Registering '{0}' event log with EntryWrittenHandeler", log.LogDisplayName.ToString());
                log.EnableRaisingEvents = true;
                log.EntryWritten += new EntryWrittenEventHandler(this.EntryHandeler);
            }

        }

        void EntryHandeler(object sender, EntryWrittenEventArgs e)
        {
            // Transform EventLog Severity from Windows to Syslog and NLog Levels
            LogLevel level;
            SyslogLevels syslogLevel;
            switch (e.Entry.EntryType)
            {
                case (EventLogEntryType.Information):
                    { level = LogLevel.Info; syslogLevel = SyslogLevels.Information; break; }
                case (EventLogEntryType.Warning):
                    { level = LogLevel.Warn; syslogLevel = SyslogLevels.Warning; break; }
                case (EventLogEntryType.Error):
                    { level = LogLevel.Error; syslogLevel = SyslogLevels.Error; break; }
                case (EventLogEntryType.SuccessAudit):
                    { level = LogLevel.Debug; syslogLevel = SyslogLevels.Debug; break; }
                case (EventLogEntryType.FailureAudit):
                    { level = LogLevel.Warn; syslogLevel = SyslogLevels.Warning; break; }
                default:
                    { level = LogLevel.Debug; syslogLevel = SyslogLevels.Debug; break; }
            }

            

           
            //Clean the message up to remote new lines.
            string message = e.Entry.Message.Replace("\r", " ").Replace("\n", " ").Replace("\t", " ") ;

            //Transform the event to be sent.
            string username;
            if (System.String.IsNullOrEmpty(e.Entry.UserName)) { username = ""; } else { username = e.Entry.UserName; }

            WindowsEventStub stubEvent = new WindowsEventStub { Category = e.Entry.Category, 
                                                                CategoryNumber = e.Entry.CategoryNumber.ToString(),
                                                                EntryType = e.Entry.EntryType.ToString(),
                                                                EventID = e.Entry.InstanceId.ToString(),
                                                                MachineName = e.Entry.MachineName,
                                                                Message = message,
                                                                Source = e.Entry.Source,
                                                                TimeGenerated = e.Entry.TimeGenerated.ToUniversalTime().ToString(),
                                                                TimeWritten = e.Entry.TimeWritten.ToUniversalTime().ToString(),
                                                                UserName = username};


            //Create a syslog message string
            string sysLogMessage = System.String.Format("<{0}>{1} {2} {3}[{4}]: {5}",
                             17 * 8 + syslogLevel, 
                             e.Entry.TimeGenerated.ToUniversalTime().ToString("MMM dd HH:mm:ss"), 
                             e.Entry.MachineName,
                             e.Entry.Source.Replace(" ", "-"),
                             e.Entry.InstanceId.ToString(), 
                             message);


            //Generate an NLog Event Object
            Logger log = LogManager.GetCurrentClassLogger();
            LogEventInfo theEvent = new LogEventInfo(level, e.Entry.Source.Replace(" ", "-"), message);

            //Add JSON properties to the event
            theEvent.Properties["JSON"] = JsonConvert.SerializeObject(stubEvent);

            //Add a formatted Syslog event to the string
            theEvent.Properties["SYSLOG"] = sysLogMessage;

            log.Log(theEvent);

        }

    }
}
