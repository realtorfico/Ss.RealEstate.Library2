using System;
using System.Net;
using System.Net.Mail;

namespace Ss.RealEstate.Library2
{
    public class SmtpMailer
    {
        const string _gmailUser = "avangari@gmail.com";
        const string _gmailPwd = "Chittu14#"; 

        public static void SendMail(string from, string[] to, string subject, string body)
        {
            if (string.IsNullOrEmpty(from) || to.Length <= 0 || string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(body)) return; 
             
            var msg = new MailMessage()
            {
                From = new MailAddress(from),
                Subject = subject,
                Body = body
            }; 
            foreach(var str in to) { msg.To.Add(str);  }

            var client = new SmtpClient()
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_gmailUser, _gmailPwd),
                Timeout = 20000
            };

            try
            {
                client.Send(msg);
            }
            catch (Exception ex)
            {
                //Eat the exception for now
            }
            finally
            {
                msg.Dispose();
            }
        }
    }
}
