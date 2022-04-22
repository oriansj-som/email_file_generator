using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using common;

namespace Email_File_generator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("starting Email_File_generator");
            
            List<Attachment> f = new List<Attachment>();
            bool fire_email = false;
            bool html_email = false;
            bool html_message = false;
            string message = "";
            string sig = null;
            string subject = "";
            string sender = "";
            List<string> message_recipients = new List<string>();
            List<string> message_cc = new List<string>();
            List<string> message_bcc = new List<string>();
            string pickup_folder = @"C:\";

            int i = 0;
            while (i < args.Length)
            {
                if (match("--attach-file", args[i]))
                {
                    Attachment a = new Attachment(args[i + 1], MediaTypeNames.Application.Octet);
                    f.Add(a);
                    i = i + 2;
                }
                else if (match("--message-plain", args[i]))
                {
                    message = args[i + 1];
                    message = message.Replace("\\n", "\r\n");
                    i = i + 2;
                }
                else if (match("--message-html", args[i]))
                {
                    message = args[i + 1];
                    html_email = true;
                    html_message = true;
                    i = i + 2;
                }
                else if (match("--message-file-plain", args[i]))
                {
                    StreamReader fileReader = new StreamReader(args[i + 1]);
                    message = fileReader.ReadToEnd();
                    i = i + 2;
                }
                else if (match("--message-file-html", args[i]))
                {
                    StreamReader fileReader = new StreamReader(args[i + 1]);
                    message = fileReader.ReadToEnd();
                    html_email = true;
                    html_message = true;
                    i = i + 2;
                }
                else if (match("--subject", args[i]))
                {
                    subject = args[i + 1];
                    i = i + 2;
                }
                else if (match("--from", args[i]))
                {
                    sender = args[i + 1];
                    i = i + 2;
                }
                else if (match("--to", args[i]))
                {
                    message_recipients.Add(args[i + 1]);
                    i = i + 2;
                }
                else if (match("--cc", args[i]))
                {
                    message_cc.Add(args[i + 1]);
                    i = i + 2;
                }
                else if (match("--bcc", args[i]))
                {
                    message_bcc.Add(args[i + 1]);
                    if (!fire_email) Console.WriteLine("Warning bcc does not survive writing to a file");
                    i = i + 2;
                }
                else if (match("--pickup-folder", args[i]))
                {
                    pickup_folder = args[i + 1];
                    i = i + 2;
                }
                else if (match("--sig-file", args[i]))
                {
                    StreamReader fileReader = new StreamReader(args[i + 1]);
                    sig = fileReader.ReadToEnd();

                    html_email = true;
                    i = i + 2;
                }
                else if (match("--fire-email", args[i]))
                {
                    fire_email = true;
                    i = i + 1;
                }
                else if (match("--verbose", args[i]))
                {
                    int index = 0;
                    foreach (string s in args)
                    {
                        Console.WriteLine(string.Format("argument {0}: {1}", index, s));
                        index = index + 1;
                    }
                    i = i + 1;
                }
                else
                {
                    Console.WriteLine(string.Format("Unknown argument: {0} received", args[i]));
                    i = i + 1;
                }
            }

            if(null != sig)
            {
                if (html_message) message = message + sig;
                else message = "<body>" + message.Replace("\n", "\n<br>") + sig + "</body>";
            }

            generate(sender, message_recipients, message_cc, message_bcc, subject, html_email, message, f, fire_email, pickup_folder);
            Console.WriteLine("email successfully generated\n");
        }

        static void generate(string from, List<string> To, List<string> cc, List<string> bcc, string subject, bool html_email, string message, List<Attachment> files, bool fire, string pickup)
        {
            Mail m = new Mail();
            m.From = from;
            m.To = To;
            m.Cc = cc;
            m.Bcc = bcc;
            m.Subject = subject;
            m.html = html_email;
            m.Message = message;
            m.Data = files;
            m.Pickup_Folder = pickup;
            bool success = m.Send(fire);
            if(!success)
            {
                if (fire) Console.WriteLine("Failed to send email");
                else Console.WriteLine("Failed to generate email file");
                Environment.Exit(4);
            }
        }

        static bool match(string a, string b)
        {
            return a.Equals(b, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
