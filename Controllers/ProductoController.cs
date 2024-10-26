using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using healthycannab.Models;
using healthycannab.Data;
using Microsoft.EntityFrameworkCore;
using healthycannab.Service;

namespace healthycannab.Controllers
{
    // [Route("api/[controller]")]
    // [ApiController]
    public class ProductoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ProductoService _productoService;
        //private readonly ILogger<ProductoController> _logger;

        /*public ProductoController(ILogger<ProductoController> logger)
        {
            _logger = logger;
        }*/

        public ProductoController(ApplicationDbContext context, ProductoService productoService)
        {
            _context = context;
            _productoService= productoService;
        }

        // public IActionResult Index()
        // {
        //     var productos = new List<Producto>
        // {
        //     new Producto { Id = 1, Nombre = "Producto 1", Descripcion = "Aceite de cannabis 100% puro", Precio = 150, ImagenUrl = "/img/a.png" },
        //     new Producto { Id = 2, Nombre = "Producto 2", Descripcion = "Unguento de cannabis", Precio = 120, ImagenUrl = "/img/b.jpg" },
        //     new Producto { Id = 3, Nombre = "Producto 3", Descripcion = "Cannabis CBD cannabidiol", Precio = 95, ImagenUrl = "/img/canabis3.png" },
        //     new Producto { Id = 3, Nombre = "Producto 4", Descripcion = "Cannabis To you", Precio = 105, ImagenUrl = "/img/canabis4.png" },
        //     new Producto { Id = 3, Nombre = "Producto 5", Descripcion = "Cannabis Gotas", Precio = 99, ImagenUrl = "/img/cannabisGotas.jpg" }
        // };
        //     return View(productos);
        // }

        public async Task<IActionResult> Index()
        {
            var productos = await _productoService.GetAll();
            return View(productos);
        }

        // public async Task<IActionResult> Index()
        // {
        //     var productos = await _productoService.GetAll();
        //     return productos != null ?
        //                 View(productos) :
        //                 Problem("Entity set 'ApplicationDbContext.DataProducto'  is null.");
        // }

        //FindAll
        [HttpGet("All")]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
        {
            return await _context.DataProducto.ToListAsync();
        }

        //findById
        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetProducto(int id)
        {
            var producto = await _context.DataProducto.FindAsync(id);

            if (producto == null)
            {
                return NotFound();
            }
            return producto;
        }

        //Create
        [HttpPost("Create")]
        public async Task<ActionResult<Producto>> PostProducto(Producto producto)
        {
            _context.DataProducto.Add(producto);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProducto", new {id = producto.Id}, producto);
        }

        //Update
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProducto(int id, Producto producto)
        {
            if (id != producto.Id)
            {
                return BadRequest();
            }

            _context.Entry(producto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        //Delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            var producto = await _context.DataProducto.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            _context.DataProducto.Remove(producto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductoExists(int id)
        {
            return _context.DataProducto.Any(e => e.Id == id);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }


        public IActionResult Chat()
        {
            return View();
        }

    }
}