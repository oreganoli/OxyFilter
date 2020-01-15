using System;
using System.Linq;
using MailKit;
using MailKit.Net.Imap;

namespace OxyFilter
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
            Username = Util.ForceGetEnv("O2_USERNAME");
            Password = Util.ForceGetEnv("O2_PASSWORD");
            try
            {
                var period = Util.ForceGetEnv("WAIT_PERIOD");
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
            Client = new ImapClient();
            Epoch = DateTime.UtcNow;
        }

        public void Authenticate()
        {
            try
            {
                if (!Client.IsConnected)
                {
                    Console.WriteLine("(Re-)connecting..."); 
                    Client.Connect(SERVER_URL, PORT);
                    Console.WriteLine("Connected.");
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
                    Console.WriteLine("(Re-)authenticating...");
                    Client.Authenticate(Username, Password);
                    Console.WriteLine("Authenticated.");
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
                    LastDelete = DateTime.UtcNow;
                }

                inbox.Expunge();
                
            }
            catch (ProtocolException ex)
            {
                Console.WriteLine("Protocol error!");
                Console.WriteLine(ex.Message);
            }
        }

        public void Disconnect()
        {
            Console.WriteLine("Disconnecting...");
            Client.Disconnect(true);
            Console.WriteLine("Disconnected.");
        }
    }
}