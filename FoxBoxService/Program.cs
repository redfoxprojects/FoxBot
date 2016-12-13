using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace FoxBoxService
{
    
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        ///
        
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            Service1 service1 = new Service1();
            
            ServicesToRun = new ServiceBase[] { service1 };
            ServiceBase.Run(ServicesToRun);
        }
    }

    
}
