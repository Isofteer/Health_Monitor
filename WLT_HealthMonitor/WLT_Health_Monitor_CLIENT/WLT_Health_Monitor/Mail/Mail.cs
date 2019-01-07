using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace WLT_Health_Monitor.Mail
{
    public class EMail
    {

        public EMail() { }
        public bool Send (string to,string body) {


            MailMessage mail = new MailMessage();

            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

            mail.From = new MailAddress("alerts@trackingalerts.net");
            mail.To.Add(to);
            mail.Subject = "WLT Health Monitoring info";
            mail.Body = body;

            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential("alerts@trackingalerts.net", "trackingalerts");
            SmtpServer.EnableSsl = true;

        
           
            try {
                SmtpServer.Send(mail);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

         
        }
    }
}
