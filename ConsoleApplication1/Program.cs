using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FoxBot;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            Bot foxBot = new Bot("FoxBot", "codingfurs", "irc.furnet.org");
            foxBot.Connect();
        }
    }
}
