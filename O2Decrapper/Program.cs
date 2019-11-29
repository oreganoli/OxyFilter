using System;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security;
using System.Threading;
using MailKit;
using MailKit.Net.Imap;

namespace O2Decrapper
{
    class Program
    {
        private const string ServerUrl = "poczta.o2.pl";
        private const int Port = 993;
        private static readonly string Username = Environment.GetEnvironmentVariable("O2_USERNAME");
        private static readonly string Password = Environment.GetEnvironmentVariable("O2_PASSWORD");

        private static void Main()
        {
            if (Username == null || Password == null)
            {
                Console.WriteLine("The O2_USERNAME and O2_PASSWORD environment variables must be set.");
                Environment.Exit(1);
            }

            var client = new ImapClient();
            client.Connect(ServerUrl, Port);
            client.Authenticate(Username, Password);
            var inbox = client.GetFolder("Inbox");
            inbox.Open(FolderAccess.ReadWrite);
            var o2 = inbox.Where(x => x.From.ToString().Contains("/o2")).ToArray();
            for (int i = 0; i < o2.Length; i++)
            {
                var msg = o2[i];
                Console.WriteLine($"FROM: {msg.From} SUBJECT: {msg.Subject}");
            }
        }
    }
}