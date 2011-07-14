using System;
using System.Collections;
using System.Configuration.Install;
using System.ServiceProcess;
using System.ComponentModel;

namespace WinLog
{
    [RunInstaller(true)]
    public class WinLogInstaller : Installer
    {
        private ServiceInstaller serviceInstaller;
        private ServiceProcessInstaller processInstaller;

        public WinLogInstaller()
        {
            serviceInstaller = new ServiceInstaller();
            processInstaller = new ServiceProcessInstaller();
            
            processInstaller.Account = ServiceAccount.LocalSystem;

            serviceInstaller.StartType = ServiceStartMode.Automatic;
            serviceInstaller.ServiceName= "WinLog";

            Installers.Add(serviceInstaller);
            Installers.Add(processInstaller);
        }

    }
}
