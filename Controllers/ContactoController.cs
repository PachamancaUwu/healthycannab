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
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using healthycannab.Services;
using ClasificacionModelo;

namespace healthycannab.Controllers
{
    
    public class ContactoController : Controller
    {
        private readonly ILogger<ContactoController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly EmailSendService _emailSend;

        public ContactoController(ILogger<ContactoController> logger, ApplicationDbContext context, EmailSendService emailSend)
        {
            _logger = logger;
            _context = context;
            _emailSend = emailSend;
        }

        
        [HttpGet]
        public async Task<IActionResult> Contacto()
        {
            var usuario = await _context.DataUsuario.FirstOrDefaultAsync(c => c.Correo == User.Identity.Name);
            var contacto = new Contacto();
            if (User.Identity.IsAuthenticated)
            {
                contacto.Correo = usuario.Correo;
                contacto.Celular = usuario.Celular;
                contacto.Nombre = usuario.Nombre;
            }
            return View(contacto);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contacto(Contacto contacto)
        {
            if (ModelState.IsValid) 
            {

                _context.Add(contacto); // Agrega el modelo al contexto
                await _context.SaveChangesAsync(); // Guarda los cambios en la base de datos

                // Mensaje de éxito
                ViewBag.Message = "Gracias por contactarnos. Te responderemos pronto.";
                
                MLModelTextClassification.ModelInput sampleData = new MLModelTextClassification.ModelInput()
                {
                    Comentario = contacto.Mensaje
                };
                MLModelTextClassification.ModelOutput output = MLModelTextClassification.Predict(sampleData);

                var sortedScoresWithLabel = MLModelTextClassification.PredictAllLabels(sampleData);
               
                var scoreKeyFirst = sortedScoresWithLabel.ToList()[0].Key;
                var scoreValueFirst = sortedScoresWithLabel.ToList()[0].Value;
                var scoreKeySecond = sortedScoresWithLabel.ToList()[1].Key;
                var scoreValueSecond = sortedScoresWithLabel.ToList()[1].Value;
                
                Console.WriteLine($"{scoreKeyFirst,-40}{scoreValueFirst,-20}");
                Console.WriteLine($"{scoreKeySecond,-40}{scoreValueSecond,-20}");

                if(scoreKeyFirst == "1")
                {
                    if(scoreValueFirst > scoreValueSecond)
                    {
                        string mensaje = $"Nombre: {contacto.Nombre}\nCorreo: {contacto.Correo}\nComentario: {contacto.Mensaje}";
                        await _emailSend.SendEmailAsync("Nuevo Comentario de Contáctanos", mensaje);
                    }
                }
                

                ModelState.Clear(); 
                return RedirectToAction("Contacto");
                
            }
            // Si el modelo no es válido, vuelve a mostrar el formulario con los mensajes de error
            return View(contacto);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}