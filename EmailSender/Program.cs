﻿using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;

namespace EmailSender
{
    class Program
    {
        static IConfiguration Configuration { get; set; }

        static void Main(string[] args)
        {
            Configure();
            var mailRecipients = new List<MailAddress>()
            {
                new MailAddress(Configuration["MailAdresses:To:1"]),
                new MailAddress(Configuration["MailAdresses:To:2"]),
                new MailAddress(Configuration["MailAdresses:To:3"]),
                new MailAddress(Configuration["MailAdresses:To:4"]),
            };
            var senderEmail = Configuration["MailAdresses:From:Email"];
            var senderName = Configuration["MailAdresses:From:Name"];
            var mailFrom = new MailAddress(senderEmail, senderName);
            var smtpHost = Configuration["SmtpClient:Host"];
            var smtpPort = Convert.ToInt32(Configuration["SmtpClient:Port"]);
            var smtp = new SmtpClient(smtpHost, smtpPort);
            var subjectMessage = "Sending data from the database";
            Console.WriteLine($"{subjectMessage}");
            Console.WriteLine($"Departure to - {smtp.Host}");
            Console.WriteLine($"Sender: name - {mailFrom.Address}, address - {mailFrom.DisplayName}");
            var users = GetUsers();
            var mail = new MailMessage();
            mail.From = mailFrom;
            Console.WriteLine("Generating a mail for users:");
            foreach (var mailTo in mailRecipients)
            {
                mail.To.Add(mailTo);
                Console.WriteLine($"Address - {mailTo.Address}");
            }

            foreach (var user in users)
            {
                var message = $"<p> Name - {user} </p>";
                mail.Body += message;
            }

            var password = Configuration["MailAdresses:From:Password"];
            smtp.Credentials = new NetworkCredential(mailFrom.Address, password);
            mail.Subject = subjectMessage;
            mail.IsBodyHtml = true;
            smtp.EnableSsl = true;
            smtp.Send(mail);
            Console.WriteLine("Messages have been sent!");
        }
        private static List<string> GetUsers()
        {
            using (var db = new EmailSenderDbContext())
            {
                var users = new List<string>();
                foreach (var user in db.Users)
                {
                    users.Add(user.Name);
                }
                return users;
            }
        }
            
        private static void Configure()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json")
                   .AddEnvironmentVariables()
                   .AddUserSecrets<Program>();
            var configuration = builder.Build();

            if (string.IsNullOrEmpty(configuration["ConnectionStrings:EmailSender.Db"]))
            {
                Console.WriteLine("Connection string not found.");
                Console.WriteLine("Please set the 'ConnectionString' environment variable to a valid");
                return;
            }

            if (string.IsNullOrEmpty(configuration["SmtpClient:Port"]))
            {
                Console.WriteLine("Port for smtp client not found.");
                Console.WriteLine("Please set the 'Port' for 'SmtpClient' environment variable to a valid");
                return;
            }

            if (string.IsNullOrEmpty(configuration["SmtpClient:Host"]))
            {
                Console.WriteLine("Host for smtp client not found.");
                Console.WriteLine("Please set the 'Port' for 'SmtpClient' environment variable to a valid");
                return;
            }

            Configuration = builder.Build();
        }
    }
}
