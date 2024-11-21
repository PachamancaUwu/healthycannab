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
using Microsoft.EntityFrameworkCore;
namespace healthycannab.Controllers
{
    public class CarritoComprasController : Controller
    {
        private readonly PayPalService _payPalService;
        private readonly ApplicationDbContext _context;
        private readonly EmailSendService _emailSendService;
        private static CarritoCompras _carrito = new CarritoCompras();

        public CarritoComprasController(ApplicationDbContext context, IConfiguration configuration, EmailSendService emailSendService)
        {
            _context = context;
            _payPalService = new PayPalService(configuration); // Inicializar PayPalService aquí
            _emailSendService = emailSendService;
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
            var usuario = await _context.DataUsuario.FindAsync(int.Parse(usuarioId));  

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
            var cantidadTotal = 0;
            List<string> detallesCarrito = new List<string>();

            foreach (var item in _carrito.Items)
            {
                detallesCarrito.Add($"Producto: {item.Producto.Nombre}, Precio: ${item.Producto.Precio}, Cantidad: {item.Cantidad}");

                var detalle = new DetallePrecio
                {
                    PedidoId = pedido.Id, 
                    ProductoId = item.ProductoId,
                    Cantidad = item.Cantidad
                };
                
                _context.DataDetallePrecio.Add(detalle);

                cantidadTotal+=item.Cantidad; // Suma la cantidad total de productos comprados
            }
            
            string detalles = string.Join("\n", detallesCarrito);
            string mensaje = $"Hola {usuario.Nombre} {usuario.ApellidoPaterno} {usuario.ApellidoMaterno} (DNI: {usuario.Dni}),\n\nGracias por tu compra en HealthyCannab. Tu pedido ha sido procesado exitosamente y será llevado a la dirección: {usuario.Dirección}.\n\nProductos adquiridos:\n{detalles}\n\nTotal: {pedido.Total}\nFecha de compra: {pedido.Fecha}\n\n¡Gracias por elegirnos!";
            await _emailSendService.SendEmailAsync("Compra en HealthyCannab", mensaje, usuario.Correo);

            //Si la cantidad de productos es mayor o igual a 4 se generará un código de descuento
            if (cantidadTotal >= 4)
            {
                bool existe;
                string partialCodigo;

                // Verifica si el código generado ya existe
                do
                {
                    partialCodigo = CodigoPromocion.GenerarCodigo();
                    existe = await _context.DataCodigoPromocion.AnyAsync(p => p.Codigo == partialCodigo);
                }while (existe);

                var codigoPromocion = new CodigoPromocion
                {
                    Codigo = partialCodigo,
                    Descuento = 0.15m,
                    FechaExpiracion = DateTime.UtcNow.AddMonths(1)
                };

                _context.DataCodigoPromocion.Add(codigoPromocion);
                
                string mensajeCodigo = $"Usted ha realizado una compra de {cantidadTotal} productos en total.\n¡Por lo que se lleva un código de descuento de un 15% de UN SOLO USO para su próxima compra en las tiendas de HealthyCannab!\n\nCódigo:{codigoPromocion.Codigo}\nFecha de caducidad: {codigoPromocion.FechaExpiracion:dd/MM/yyyy}";
                await _emailSendService.SendEmailAsync("Código de descuento por su compra en HealthyCannab", mensajeCodigo, usuario.Correo);
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


