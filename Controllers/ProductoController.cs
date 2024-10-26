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
    //[Route("[controller]")]
    public class ProductoController : Controller
    {
        //private readonly ILogger<ProductoController> _logger;

        /*public ProductoController(ILogger<ProductoController> logger)
        {
            _logger = logger;
        }*/

        private readonly ApplicationDbContext _context;

        public ProductoController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var productos = _context.DataProducto.ToList();
            return View(productos);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}