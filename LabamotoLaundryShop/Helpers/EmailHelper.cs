using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;


namespace LabamotoLaundryShop.Helpers
{
    public static class EmailHelper
    {
        public static void SendEmail(string toName, string toEmail, string subject, string body)
        {
            // Example using Gmail SMTP
            var fromAddress = new MailAddress("jhoncarlosuan26@gmail.com", "Labamoto Laundry");
            var toAddress = new MailAddress(toEmail, toName);
            const string fromPassword = "tvqyqwjmlswfdjrc"; // use app password

            using (var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword),
                Timeout = 20000
            })
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }
    }
}