using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace CrossLinkAPIService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Service1()
            };
            ServiceBase.Run(ServicesToRun);

            //Service1 obj = new Service1();
            //obj.UpdateBankStatus();

            //Test ts = new CrossLinkAPIService.Test();
            //ts.getUserIdList();
        }
    }
}
