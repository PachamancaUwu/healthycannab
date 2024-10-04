using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using healthycannab.Models;
using Microsoft.AspNetCore.Identity;
using healthycannab.Data;


namespace healthycannab.Controllers
{
    
    public class RegistroController : Controller
    {
        private readonly ApplicationDbContext _context; // Cambia a ApplicationDbContext

        public RegistroController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registrar(Usuario model)
        {
            if (ModelState.IsValid)
            {
                // Verificar si ya existe un usuario con el mismo correo
                var existingUser = _context.DataUsuario.FirstOrDefault(u => u.Correo == model.Correo);
                if (existingUser == null)
                {
                    // Agregar el nuevo usuario a la base de datos
                    _context.DataUsuario.Add(model);
                    _context.SaveChanges(); // Guardar cambios en la base de datos

                    ViewBag.SuccessMessage = "Usuario registrado correctamente.";
                    return View();
                }
                else
                {
                    ViewBag.ErrorMessage = "El correo ya está registrado.";
                }
            }
            return View(model);
        }
    }
}