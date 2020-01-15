using System.Threading;

namespace O2Decrapper
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
        }
    }
}