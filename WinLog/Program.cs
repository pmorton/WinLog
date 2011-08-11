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
            WinLogService svc = new WinLogService();

            if (Environment.UserInteractive)
            {
                svc.Start(args);
                Console.WriteLine("Press any key to stop the service");
                Console.Read();
                svc.Stop();
            }

            ServiceBase.Run(new WinLogService());
        }
    }
}
