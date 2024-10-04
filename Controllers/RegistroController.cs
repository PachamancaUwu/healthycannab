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
        private readonly ApplicationDbContext _context;

        public RegistroController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registro(Usuario model)
        {
            if (ModelState.IsValid)
            {
                
                var existingUser = _context.DataUsuario.FirstOrDefault(u => u.Correo == model.Correo);
                if (existingUser == null)
                {
                    
                    _context.DataUsuario.Add(model);
                    _context.SaveChanges();

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