using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using healthycannab.Models;
using healthycannab.Data;

namespace healthycannab.Controllers
{
   public class InicioSesionController : Controller
{
    private readonly ApplicationDbContext _context;

    public InicioSesionController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Acción GET para mostrar la vista de registro
    [HttpGet]
    public IActionResult Registro()
    {
        return View();
    }

    // Acción POST para registrar un nuevo usuario
    [HttpPost]
    public async Task<IActionResult> Registro(Usuario usuario)
    {
        var existeUsuario = await _context.DataUsuario.FirstOrDefaultAsync(u => u.Correo == usuario.Correo);

        if (existeUsuario == null)
        {
            usuario.Rol = "Usuario";

            _context.Add(usuario);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");  
        }
        else
        {
            ViewBag.Error = "El correo ya está registrado.";
            return View(usuario);
        }
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string correo, string contrasena)
    {
        if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(contrasena))
        {
            ViewBag.Error = "Por favor, complete todos los campos.";
            return View();
        }

        var usuario = await _context.DataUsuario
            .FirstOrDefaultAsync(u => u.Correo == correo && u.Contrasena == contrasena);

        if (usuario != null)
        {
            if (usuario.Rol == "Administrador")
            {
                return RedirectToAction("Index", "Admin");  
            }
            return RedirectToAction("Index", "Home");  
        }
        else
        {
            ViewBag.Error = "Credenciales incorrectas.";
        }

        return View();
    }

    public IActionResult Logout()
    {
        return RedirectToAction("Login");
    }
}

}
