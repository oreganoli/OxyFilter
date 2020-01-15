namespace O2Decrapper
{
    public class InfoService : IInfoService
    {
        public Info Get()
        {
            Info info;
            lock (Program.Handler)
            {
                info = new Info(Program.Handler.Epoch.ToString("o"), Program.Handler.LastDelete.ToString("o"),
                    Program.Handler.Counter);
            }
            return info;
        }
    }
}