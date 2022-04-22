using System;
using System.Collections.Generic;
using System.Net.Mail;

namespace common
{
    internal class Mail
    {

        private MailAddress from;
        public bool html;
        private List<string> to;
        private List<string> cc;
        private List<string> bcc;
        private string subject;
        private string message;
        private string pickup_folder;
        private List<Attachment> data;

        public List<string> To
        {
            get { return to; }
            set { to = value; }
        }

        public List<string> Cc
        {
            get { return cc; }
            set { cc = value; }
        }

        public List<string> Bcc
        {
            get { return bcc; }
            set { bcc = value; }
        }

        public List<Attachment> Data
        {
            get { return data; }
            set { data = value; }
        }

        public string Pickup_Folder
        {
            get { return pickup_folder; }
            set { pickup_folder = value; }
        }

        public string From
        {
            get { return from.ToString(); }
            set { from = new MailAddress(value); }
        }

        public string Subject
        {
            get { return subject; }
            set { subject = value; }
        }

        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        public bool Send(bool fire_email)
        {
            try
            {
                //create mail message object
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = from;
                foreach (string s in to)
                {
                    mailMessage.To.Add(s);         //add to recepient address
                }
                foreach (string s in cc)
                {
                    mailMessage.CC.Add(s);
                }
                foreach (string s in bcc)
                {
                    mailMessage.Bcc.Add(s);
                }
                mailMessage.Subject = subject;  //set subject 
                mailMessage.Body = message;     //set message body 
                mailMessage.IsBodyHtml = html; //set message type
                foreach (Attachment a in data)
                {
                    mailMessage.Attachments.Add(a);
                }
                
                if (!fire_email) mailMessage.Headers.Add("X-Unsent", "1");

                //create SMTP client object to send the message
                SmtpClient smtpMail = new SmtpClient();
                if(!fire_email)
                {
                    smtpMail.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    smtpMail.PickupDirectoryLocation = pickup_folder;
                }
                else
                {
                    /* setup email server here */
                    smtpMail.Host = "you should know";
                    smtpMail.Port = 6666666;
                }
                smtpMail.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }
    }
}