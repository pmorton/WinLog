using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinLog
{
    class WindowsEventStub
    {
        public string Category { get; set; } 
        public string CategoryNumber { get; set; }
        public string EntryType { get; set; }
        public string EventID { get; set; }
        public string MachineName { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
        public string TimeGenerated { get; set; }
        public string TimeWritten { get; set; }
        public string UserName { get; set; } 
    }
}
