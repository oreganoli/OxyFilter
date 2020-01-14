using System;
using System.IO;
using System.Linq;
using System.Threading;
using MailKit;
using MailKit.Net.Imap;

namespace O2Decrapper
{
    public class O2Handler
    {
        private const string O2_SUFFIX = "/o2"; // All O2 spam messages have this in their From field.
        private const string SERVER_URL = "poczta.o2.pl";
        private const int PORT = 993;
        private readonly string Username;
        private readonly string Password;
        public readonly int WaitPeriod;
        private IMailStore Client;
        public readonly DateTime Epoch; // When the handler started functioning.
        public DateTime LastDelete { get; private set; }
        public int Counter { get; private set; } // How many messages the handler has deleted since startup.

        public O2Handler()
        {
            Username = Environment.GetEnvironmentVariable("O2_USERNAME");
            Password = Environment.GetEnvironmentVariable("O2_PASSWORD");
            try
            {
                var period = Environment.GetEnvironmentVariable("WAIT_PERIOD");
                if (period == null)
                {
                    Console.WriteLine("The WAIT_PERIOD environment variable must be set.");
                    Environment.Exit(1);
                }

                WaitPeriod = int.Parse(period);
                if (WaitPeriod <= 0)
                {
                    Console.WriteLine("The WAIT_PERIOD environment variable must be a positive 32-bit signed integer.");
                    Environment.Exit(1);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("The WAIT_PERIOD environment variable must be a positive 32-bit signed integer.");
                Environment.Exit(1);
            }
            if (Username == null || Password == null)
            {
                Console.WriteLine("The O2_USERNAME and O2_PASSWORD environment variables must be set.");
                Environment.Exit(1);
            }
            Client = new ImapClient();
            Epoch = DateTime.UtcNow;
        }

        public void Authenticate()
        {
            try
            {
                if (!Client.IsConnected)
                {
                    Client.Connect(SERVER_URL, PORT);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("The application could not connect to the mail server. Check your internet connection.");
                Environment.Exit(2);
            }

            try
            {
                if (!Client.IsAuthenticated)
                {
                    Client.Authenticate(Username, Password);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("The application could not be authenticated. Check if you have set the correct credentials.");
                Environment.Exit(3);
            }
        }

        public void Remove()
        {
            try
            {
                var inbox = Client.Inbox;
                inbox.Open(FolderAccess.ReadWrite);
                var items = inbox.Fetch(0, -1, MessageSummaryItems.Envelope | MessageSummaryItems.UniqueId);
                var o2 = items.Where(x => x.Envelope.From.ToString().Contains(O2_SUFFIX)).ToArray();
                foreach (var msg in o2)
                {
                    Console.WriteLine(
                        $"UID: {msg.UniqueId}, From: {msg.Envelope.From}, Subject: \"{msg.Envelope.Subject}\"");
                    inbox.SetFlags(msg.UniqueId, MessageFlags.Seen | MessageFlags.Deleted, true);
                    Counter += 1;
                }

                inbox.Expunge();
                LastDelete = DateTime.UtcNow;
            }
            catch (ProtocolException ex)
            {
                Console.WriteLine("Protocol error!");
                Console.WriteLine(ex.Message);
            }
        }
    }
}