using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using healthycannab.Models;
using healthycannab.Data;

namespace healthycannab.Controllers
{
    
    public class ContactoController : BaseController
    {
       private readonly ILogger<ContactoController> _logger;
        private readonly ApplicationDbContext _context; // Contexto de la base de datos

        public ContactoController(ILogger<ContactoController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context; // Inicializa el contexto
        }

        // GET: Muestra el formulario de contacto
        [HttpGet]
        public IActionResult Contacto()
        {
            return View();
        }

        // POST: Procesa los datos del formulario de contacto y los guarda en la base de datos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contacto(Contacto model)
        {
            if (ModelState.IsValid) // Valida que el modelo sea correcto
            {
                _context.Add(model); // Agrega el modelo al contexto
                await _context.SaveChangesAsync(); // Guarda los cambios en la base de datos

                // Mensaje de éxito
                ViewBag.Message = "Gracias por contactarnos. Te responderemos pronto.";
                
                // Limpiar el modelo después de guardar
                ModelState.Clear(); // Esto limpia los datos del modelo
                return View(); // Regresa a la vista Contacto para mostrar el mensaje
            }

            // Si el modelo no es válido, vuelve a mostrar el formulario con los mensajes de error
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}