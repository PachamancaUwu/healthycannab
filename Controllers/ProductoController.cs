using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using healthycannab.Models;

namespace healthycannab.Controllers
{
    //[Route("[controller]")]
    public class ProductoController : Controller
    {
        //private readonly ILogger<ProductoController> _logger;

        /*public ProductoController(ILogger<ProductoController> logger)
        {
            _logger = logger;
        }*/

        public IActionResult Producto()
        {
            var productos = new List<Producto>
        {
            new Producto { Id = 1, Nombre = "Producto 1", Descripcion = "Aceite de cannabis 100% puro", Precio = 150, ImagenUrl = "/img/a.png" },
            new Producto { Id = 2, Nombre = "Producto 2", Descripcion = "Unguento de cannabis", Precio = 120, ImagenUrl = "/img/b.jpg" },
            new Producto { Id = 3, Nombre = "Producto 3", Descripcion = "Cannabis CBD cannabidiol", Precio = 95, ImagenUrl = "/img/canabis3.png" },
            new Producto { Id = 3, Nombre = "Producto 4", Descripcion = "Cannabis To you", Precio = 105, ImagenUrl = "/img/canabis4.png" },
            new Producto { Id = 3, Nombre = "Producto 5", Descripcion = "Cannabis Gotas", Precio = 99, ImagenUrl = "/img/cannabisGotas.jpg" }
        };
            return View(productos);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}