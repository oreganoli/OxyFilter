using System;
using System.IO;
using System.Net;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
namespace OxyFilter
{
    internal static class Program
    {
        public static readonly O2Handler Handler = new O2Handler();
        private static void Main()
        {
            new Thread(() => {
                while (true)
                {
                    int period;
                    lock (Handler)
                    {
                        period = Handler.WaitPeriod;
                        Handler.Authenticate();
                        Handler.Remove();
                    }
                    Thread.Sleep(period);
                }
            }).Start();
            var host = new WebHostBuilder()
                .UseKestrel()
                .ConfigureKestrel(options => { options.Listen(IPAddress.Loopback, int.Parse(Util.ForceGetEnv("PORT"))); })
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();
            host.Start();
        }
    }
}