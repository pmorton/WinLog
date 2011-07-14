using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceProcess;

namespace WinLog
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceBase.Run(new WinLogService());
        }
    }
}
