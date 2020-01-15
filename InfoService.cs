namespace OxyFilter
{
    public class InfoService : IInfoService
    {
        public Info Get()
        {
            Info info;
            lock (Program.Handler)
            {
                info = new Info(Program.Handler.Epoch, Program.Handler.LastDelete,
                    Program.Handler.Counter);
            }
            return info;
        }
    }
}