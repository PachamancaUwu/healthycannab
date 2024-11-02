using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace healthycannab.Services
{
    public class EmailSendService
    {
        private readonly IConfiguration _config;

        public EmailSendService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string subject, string message)
        {
            // Obtén la API key desde la variable de entorno
            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new System.Exception("La API Key de SendGrid no está configurada como variable de entorno.");
            }
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("mathias_cueto@usmp.pe", "HealthyCannab");
            var to = new EmailAddress("healthycannabis0@gmail.com");
            var msg = MailHelper.CreateSingleEmail(from, to, subject, message, null);

            var response = await client.SendEmailAsync(msg);
        }
    }
}