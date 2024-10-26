using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using healthycannab.Models;
using healthycannab.Data; 

namespace healthycannab.Controllers
{
    public class CarritoComprasController : Controller
    {
        

        private readonly ApplicationDbContext _context;
        private static CarritoCompras _carrito = new CarritoCompras();

        public CarritoComprasController(ApplicationDbContext context)
        {
            _context = context;
        }


        
        [HttpPost]
        public IActionResult Agregar(int productoId)
        {
            var producto = _context.DataProducto.FirstOrDefault(p => p.Id == productoId);
            
            if (producto == null)
            {
                return NotFound();
            }

            var item = _carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
            
            if (item == null)
            {
                _carrito.Items.Add(new ItemCarrito { ProductoId = productoId, Producto = producto, Cantidad = 1 });
            }
            else
            {
                item.Cantidad++;
            }

            return RedirectToAction("Index", "Producto");
        }

        // Método para ver el carrito
        public IActionResult CarritoCompras()
        {
            return View(_carrito); 
        }

        // Método para simular el pago
        public IActionResult SimularPago()
        {
            // Aquí puedes redirigir a PayPal o mostrar un mensaje de éxito
            return Content("Pago simulado exitosamente.");
        }

        public IActionResult VaciarCarrito()
        {
            _carrito.Items.Clear();
            return RedirectToAction("Index");
        }

        
       [HttpPost]
        public IActionResult ActualizarCantidad(int productoId, int cantidad)
        {
            var item = _carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
            
            if (item != null)
            {
                item.Cantidad = cantidad;
            }

            return RedirectToAction("CarritoCompras");
        }

       
         public IActionResult Eliminar(int productoId)
        {
            var item = _carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
            
            if (item != null)
            {
                _carrito.Items.Remove(item);
            }

            return RedirectToAction("CarritoCompras");
        }

    }
}
