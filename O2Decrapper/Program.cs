using System;
using System.Linq;
using System.Threading;
using MailKit;
using MailKit.Net.Imap;

namespace O2Decrapper
{
    internal static class Program
    {
        private static void Main()
        {
            var handler = new O2Handler();
            new Thread(() => {
                while (true)
                {
                    lock (handler)
                    {
                        handler.Authenticate();
                        handler.Remove();
                    }
                    Thread.Sleep(handler.WaitPeriod);
                }
            }).Start();
            Thread.Sleep(10000);
            Console.WriteLine(handler.LastDelete);
        }
    }
}