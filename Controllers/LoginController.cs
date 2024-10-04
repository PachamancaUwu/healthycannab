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
    
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private readonly ApplicationDbContext _context;

        public LoginController(ILogger<LoginController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(Usuario model)
        {
            if (ModelState.IsValid)
            {
                // Verifica si las credenciales son correctas
                var user = _context.DataUsuario.FirstOrDefault(u => u.Correo == model.Correo && u.Contrasena == model.Contrasena);
                if (user != null)
                {
                    // Inicio de sesión exitoso
                    TempData["SuccessMessage"] = "Inicio de sesión exitoso."; // Almacena en TempData
                    return RedirectToAction("Inicio", "Main");
                }
                else
                {
                    // Credenciales incorrectas
                    TempData["ErrorMessage"] = "Correo o contraseña incorrectos."; // Almacena en TempData
                }
            }
            // Si llegamos aquí, algo falló, volvemos a mostrar la vista de inicio de sesión con el modelo
            return View(model);
        }
    }
}