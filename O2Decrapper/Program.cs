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
        static readonly string SERVER_URL = "poczta.o2.pl";

        static void Main(string[] args)
        {
            var username = Environment.GetEnvironmentVariable("O2_USERNAME");
            var password = Environment.GetEnvironmentVariable("O2_PASSWORD");
            if (username == null || password == null)
            {
                Console.WriteLine("The O2_USERNAME and O2_PASSWORD environment variables must be set.");
                Environment.Exit(1);
            }

            var client = new ImapClient();
            client.Connect(SERVER_URL, 993);
            client.Authenticate(username, password);
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