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
using healthycannab.Services;
using Microsoft.AspNetCore.Authorization;

namespace healthycannab.Controllers
{

    public class ProductoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ProductoService _productoService;

        public ProductoController(ApplicationDbContext context, ProductoService productoService)
        {
            _context = context;
            _productoService= productoService;
        }

        public async Task<IActionResult> Producto()
        {
            var productos = await _productoService.GetAll();
            return View(productos);
        }

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


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }


        public IActionResult Chat()
        {
            return View();
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AgregarComentario(int productoId, string contenido)
        {
            var comentario = new Comentario
            {
                ProductoId = productoId,
                Contenido = contenido,
                Usuario = User.Identity.Name,
                Fecha = DateTime.UtcNow //formato UTC aceptado por postgres
            };

            _context.DataComentario.Add(comentario);
            await _context.SaveChangesAsync();

            return RedirectToAction("DetalleCompleto", new { id = productoId });
        }



        //filtro
        //[HttpGet("Producto/Filtrar")]
        public async Task<IActionResult> ListarProductos(string? nombre, decimal? precioMin, decimal? precioMax)
        {
            // Llamada al servicio para obtener productos filtrados
            var productos = await _productoService.FilterProductos(nombre, precioMin, precioMax);

            // Pasar los parámetros al ViewData para que la vista recuerde los valores ingresados
            ViewData["nombre"] = nombre;
            ViewData["precioMin"] = precioMin;
            ViewData["precioMax"] = precioMax;

            return View("Producto", productos);
        }

        //Detalle completo del producto
        public async Task<IActionResult> DetalleCompleto(int id)
        {
             var producto = await _productoService.GetProductoById(id);
            if (producto == null)
            {
                return NotFound();
            }

            var comentarios = await _context.DataComentario
                .Where(c => c.ProductoId == id)
                .ToListAsync();

            var viewModel = new ProductoDetalleViewModel
            {
                Producto = producto,
                Comentarios = comentarios,
                NuevoComentario = new Comentario()  // Inicializa un nuevo comentario vacío
            };

            return View(viewModel);
        }





        [Authorize(Roles ="Administrador")]
        public async Task<IActionResult> AdminProducto()
        {
            var productos = await _context.DataProducto.ToListAsync();
            return View(productos);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.DataProducto
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Descripcion,Precio,ImagenUrl")] Producto producto)
        {
            if (ModelState.IsValid)
            {
                _context.Add(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(AdminProducto));
            }
            return View(producto);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.DataProducto.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            return View(producto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Descripcion,Precio,ImagenUrl")] Producto producto)
        {
            if (id != producto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(AdminProducto));
            }
            return View(producto);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.DataProducto
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int id)
        {
            var producto = await _context.DataProducto.FindAsync(id);
            if (producto != null)
            {
                _context.DataProducto.Remove(producto);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(AdminProducto));
        }


        private bool ProductoExists(int id)
        {
            return _context.DataProducto.Any(e => e.Id == id);
        }

    }
}

