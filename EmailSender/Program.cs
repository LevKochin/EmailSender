using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace EmailSender
{
    class Program
    {
        static void Main(string[] args)
        {
            var mailRecipients = new List<MailAddress>()
            {
                new MailAddress("somemail@gmail.com"),
                new MailAddress("anymail@gmail.com"),
                new MailAddress("someKindOfmail@gmail.com"),
                new MailAddress("rat.ratio@yandex.ru"),
            };
            var mailFrom = new MailAddress("rat.ratio888@gmail.com", "Lev");
            var smtp = new SmtpClient("smtp.gmail.com", 587);
            var subjectMessage = "Sending data from the database";
            Console.WriteLine($"{subjectMessage}");
            Console.WriteLine($"Departure to - {smtp.Host}");
            Console.WriteLine($"Sender: name - {mailFrom.Address}, address - {mailFrom.DisplayName}");
            using (var db = new EmailSenderDbContext())
            {
                var users = db.Users.ToList();
                Console.WriteLine("Generating a mail for users:");
                foreach (var mailTo in mailRecipients)
                {
                    Console.WriteLine($"Address - {mailTo.Address}");
                    var mail = new MailMessage(mailFrom, mailTo);
                    mail.Subject = subjectMessage;
                    mail.IsBodyHtml = true;
                    smtp.Credentials = new NetworkCredential(mailFrom.Address, "3822557455Leo");
                    foreach (var user in users)
                    {
                        var message = $"<p> Name - {user.Name} </p>";
                        mail.Body += message;
                    }
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
            }
            Console.WriteLine("Messages have been sent!");
            Console.ReadKey();
        }
    }
}
