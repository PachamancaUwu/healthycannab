using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using healthycannab.Models;
using healthycannab.Data; 
using healthycannab.Services;
using PayPal.Api;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore.Storage;
namespace healthycannab.Controllers
{
    public class CarritoComprasController : Controller
    {
        private readonly PayPalService _payPalService;
        private readonly ApplicationDbContext _context;
        private static CarritoCompras _carrito = new CarritoCompras();

        public CarritoComprasController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _payPalService = new PayPalService(configuration); // Inicializar PayPalService aquí
        }

        [HttpPost]
        public IActionResult Agregar(int productoId)
        {
            var producto = _context.DataProducto.FirstOrDefault(p => p.Id == productoId);
            
            if (producto == null)
            {
                return NotFound(); // Manejo de error si el producto no se encuentra
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

            return RedirectToAction("Producto", "Producto");
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
        
        public IActionResult CarritoCompras()
        {
            return View(_carrito); 
        }


        [HttpPost]
        public IActionResult CreatePayment()
        {
            var total = _carrito.Items.Sum(i => i.Producto.Precio * i.Cantidad);
            var currency = "USD"; // Puedes hacerlo dinámico si es necesario
            var returnUrl = Url.Action("Success", "CarritoCompras", null, Request.Scheme);
            var cancelUrl = Url.Action("Cancel", "CarritoCompras", null, Request.Scheme);

            var payment = _payPalService.CreatePayment(total, currency, returnUrl, cancelUrl);
            var approvalUrl = payment.links.FirstOrDefault(link => link.rel.Equals("approval_url", StringComparison.OrdinalIgnoreCase))?.href;

            if (approvalUrl != null)
            {
                return Redirect(approvalUrl);
            }

            return View("Error");
        }

        [HttpGet]
        public async Task<IActionResult> Success(string paymentId, string token, string PayerID)
        {

            var paymentExecution = new PaymentExecution { payer_id = PayerID };
            var payment = Payment.Get(_payPalService.ApiContext, paymentId);

            var executedPayment = payment.Execute(_payPalService.ApiContext, paymentExecution);

            if (executedPayment.state.ToLower() != "approved")
            {
                return View("Error");
            }

            // Obtener el usuario actual
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(usuarioId))
            {
                return Unauthorized();
            }

            // Creamos el Pedido
            var pedido = new Pedido
            {
                Fecha = DateTime.UtcNow,
                Total = _carrito.Items.Sum(i => i.Producto.Precio * i.Cantidad), 
                UsuarioId = int.Parse(usuarioId) 
            };

        
            _context.DataPedido.Add(pedido);
            await _context.SaveChangesAsync(); 

            // Insertar en la tabla DetallePrecio
            foreach (var item in _carrito.Items)
            {
                var detalle = new DetallePrecio
                {
                    PedidoId = pedido.Id, 
                    ProductoId = item.ProductoId,
                    Cantidad = item.Cantidad
                };

                _context.DataDetallePrecio.Add(detalle);
            }

            await _context.SaveChangesAsync();

            _carrito.Items.Clear();

            return View("Success", executedPayment);
        }


        public IActionResult Cancel()
        {
            return View();
        }
    }
}


