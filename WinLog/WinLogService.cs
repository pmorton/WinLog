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
           
            //Handle Event Log Events
            EventLog[] mlogs = EventLog.GetEventLogs();

            foreach (EventLog log in mlogs)
            {
                log.EnableRaisingEvents = true;
                log.EntryWritten += new EntryWrittenEventHandler(this.EntryHandeler);
            }

        }

        void EntryHandeler(object sender, EntryWrittenEventArgs e)
        {
            // Transform EventLog Severity from Windows to Syslog
            LogLevel level;
            switch (e.Entry.EntryType)
            {
                case (EventLogEntryType.Information):
                    { level = LogLevel.Info; break; }
                case (EventLogEntryType.Warning):
                    { level = LogLevel.Warn; break; }
                case (EventLogEntryType.Error):
                    {level = LogLevel.Error; break; }
                case (EventLogEntryType.SuccessAudit):
                    { level = LogLevel.Trace; break; }
                case (EventLogEntryType.FailureAudit):
                    { level = LogLevel.Info; break; }
                default:
                    { level = LogLevel.Debug; break; }
            }

           

            string message = System.String.Format("EventID:{0} {1}",
                                                    e.Entry.EventID,
                                                    e.Entry.Message.Replace("\r", " ").Replace("\n", " ").Replace("\t", " ") );

            WindowsEventStub stubEvent = new WindowsEventStub { Category = e.Entry.Category, 
                                                                CategoryNumber = e.Entry.CategoryNumber.ToString(),
                                                                EntryType = e.Entry.EntryType.ToString(),
                                                                EventID = e.Entry.EventID.ToString(),
                                                                MachineName = e.Entry.MachineName,
                                                                Message = message,
                                                                Source = e.Entry.Source,
                                                                TimeGenerated = e.Entry.TimeGenerated.ToUniversalTime().ToString(),
                                                                TimeWritten = e.Entry.TimeWritten.ToUniversalTime().ToString(),
                                                                UserName = e.Entry.UserName};
            
            Logger log = LogManager.GetCurrentClassLogger();
            LogEventInfo theEvent = new LogEventInfo(level, e.Entry.Source.Replace(" ", "-") , message );
            
            theEvent.Properties["JSON"] = JsonConvert.SerializeObject(stubEvent);
            log.Log(theEvent);

        }

    }
}
