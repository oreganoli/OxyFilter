using System;
using System.Linq;
using System.Threading;
using MailKit;
using MailKit.Net.Imap;

namespace O2Decrapper
{
    class Program
    {
        private const string O2Suffix = "/o2";
        private const string ServerUrl = "poczta.o2.pl";
        private const int Port = 993;
        private static readonly string Username = Environment.GetEnvironmentVariable("O2_USERNAME");
        private static readonly string Password = Environment.GetEnvironmentVariable("O2_PASSWORD");

        private static void HandleO2(IMailStore client)
        {
            var inbox = client.Inbox;
            inbox.Open(FolderAccess.ReadWrite);
            Console.WriteLine("Checking the inbox...");
            var items = inbox.Fetch(0, -1, MessageSummaryItems.Envelope | MessageSummaryItems.UniqueId);
            var o2 = items.Where(x => x.Envelope.From.ToString().Contains(O2Suffix)).ToArray();
            switch (o2.Length)
            {
                case 0:
                    Console.WriteLine("No O2 crap!");
                    break;
                case 1:
                    Console.WriteLine("1 O2 crap message found!");
                    break;
                default:
                    Console.WriteLine($"{o2.Length} crap messages from O2 found!");
                    break;
            }
            foreach (var msgSum in o2)
            {
                Console.WriteLine($"UID: {msgSum.UniqueId}, From: {msgSum.Envelope.From}, Subject: \"{msgSum.Envelope.Subject}\"");
                inbox.SetFlags(msgSum.UniqueId, MessageFlags.Deleted | MessageFlags.Seen, true);
                Console.WriteLine($"Marked message with UID {msgSum.UniqueId} as seen and marked it for deletion.");
            }
            inbox.Expunge();
            Console.WriteLine("Expunged the inbox.");
        }
        private static void Main()
        {
            var periodStr = Environment.GetEnvironmentVariable("WAIT_PERIOD");
            if (Username == null || Password == null || periodStr == null)
            {
                Console.WriteLine("The O2_USERNAME, O2_PASSWORD and WAIT_PERIOD environment variables must be set.");
                Environment.Exit(1);
            }
            var parsed = int.TryParse(periodStr, out var waitPeriod);
            if (!parsed || waitPeriod < 1)
            {
                Console.WriteLine($"WAIT_PERIOD must be a positive integer expressing a number of milliseconds.");
                Environment.Exit(1);
            };
            using (var client = new ImapClient())
            {
                Console.WriteLine($"Connecting to {ServerUrl}:{Port}...");
                client.Connect(ServerUrl, Port);
                Console.WriteLine("Connected.");
                client.Authenticate(Username, Password);
                Console.WriteLine("Authenticated.");
                while (true)
                {
                    HandleO2(client);
                    Thread.Sleep(waitPeriod);
                }
            }
            
            
        }
    }
}