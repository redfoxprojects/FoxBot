using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using FoxBot;

namespace FoxBoxService
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
            this.ServiceName = "Service1";
        }

        protected override void OnStart(string[] args)
        {
            Bot bot = new Bot("FoxBot", "#channel", "irc.furnet.org");
            bot.Connect();
        }

        protected override void OnStop()
        {
        }
    }
}
