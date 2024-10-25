using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net; //Necesario para el envio de correo
using System.Net.Mail; //Necesario para el envio de correo
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using healthycannab.Models;
using healthycannab.Data;

namespace healthycannab.Controllers
{
    
    public class ContactoController : Controller
    {
       private readonly ILogger<ContactoController> _logger;
        private readonly ApplicationDbContext _context;

        public ContactoController(ILogger<ContactoController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        
        [HttpGet]
        public IActionResult Contacto()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contacto(Contacto model)
        {
            if (ModelState.IsValid) 
            {
                _context.Add(model); 
                await _context.SaveChangesAsync(); 
              //  await EnviarCorreoAsync(model); //Enviamos el correo
                
                ViewBag.Message = "Gracias por contactarnos. Te responderemos pronto.";
                
                
                ModelState.Clear(); 
                return View();
            }

            
            return View(model);
        }

        /*
        //Método para enviar el correo
        private async Task EnviarCorreoAsync(Contacto model)
            {
                var fromAddress = new MailAddress("healthycannabis0@gmail.com", "HealthyCannabis");
                var toAddress = new MailAddress(model.Correo, model.Nombre);
                const string fromPassword = "healthycannab1";
                const string subject = "Confirmación de contacto";
                string body = $"Gracias por contactarnos, {model.Nombre}. Hemos recibido tu mensaje:\n\n{model.Mensaje}";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };

                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    await smtp.SendMailAsync(message);
                }
            }

        */
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}