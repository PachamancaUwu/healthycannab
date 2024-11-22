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
        
        [HttpPost]
        public async Task<IActionResult> AplicarDescuento(string codigo){

            var codigoPromocion = await _context.DataCodigoPromocion.FirstOrDefaultAsync(c => c.Codigo == codigo);
            
            if (codigoPromocion == null)
            {
                ViewBag.Error = "El código de descuento ingresado no es válido";
                return View("CarritoCompras", _carrito);
            }
            if (codigoPromocion.FechaExpiracion <= DateTime.Now)
            {
                ViewBag.Error = "El código de descuento ingresado ya ha vencido";
                return View("CarritoCompras", _carrito);
            }
            if (codigoPromocion.Usado)
            {
                ViewBag.Error = "El código de descuento ingresado ya ha sido usado";
                return View("CarritoCompras", _carrito);
            }

            _carrito.Descuento = codigoPromocion.Descuento;
            _carrito.TotalDescuento = _carrito.Total * (1 - _carrito.Descuento);
            _carrito.Codigo = codigoPromocion.Codigo;

            return RedirectToAction("CarritoCompras");
        }

        public IActionResult EliminarDescuento(){

            _carrito.Descuento = 0;
            _carrito.TotalDescuento = 0;
            _carrito.Codigo = string.Empty;

            return RedirectToAction("CarritoCompras");
        }

        public IActionResult CarritoCompras()
        {
            return View(_carrito); 
        }


        [HttpPost]
        public IActionResult CreatePayment(decimal totalModel)
        {
            var total = totalModel;
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
            var pedido = new Pedido();
            string mensajeUsoCodigo = string.Empty;
            if (string.IsNullOrEmpty(_carrito.Codigo))
            {       
                pedido.Total = _carrito.Total;         
            }
            else
            {
                pedido.Total = _carrito.TotalDescuento;
                var codigo = await _context.DataCodigoPromocion.FirstOrDefaultAsync(c => c.Codigo == _carrito.Codigo);
                pedido.CodigoPromocionId = codigo.Id;

                //Marcar código como usado  
                codigo.Usado = true;
                _context.DataCodigoPromocion.Update(codigo);
                mensajeUsoCodigo = $"Al usar un código de descuento el precio total se redujo de ${_carrito.Total} a\n";
            }
            pedido.Fecha = DateTime.UtcNow;
            pedido.UsuarioId = int.Parse(usuarioId);
        
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

            // Formatear a zona de Perú, Lima
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(pedido.Fecha, timeZone);
            string formattedDate = localTime.ToString("dd/MM/yyyy HH:mm:ss");

            string mensaje = $"Hola {usuario.Nombre} {usuario.ApellidoPaterno} {usuario.ApellidoMaterno} (DNI: {usuario.Dni}),\n\nGracias por tu compra en HealthyCannab. Tu pedido ha sido procesado exitosamente y será llevado a la dirección: {usuario.Dirección}.\n\nProductos adquiridos:\n{detalles}\n\n{mensajeUsoCodigo}Total: ${pedido.Total}\nFecha de compra: {formattedDate}";


            //Si la cantidad de productos es mayor o igual a 4 se generará un código de descuento
            //No se genera un código si estás usando uno en tu compra actual
            if (cantidadTotal >= 5 && string.IsNullOrEmpty(_carrito.Codigo))
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
                
                string mensajeCodigo = $"Usted ha realizado una compra de {cantidadTotal} productos en total.\n¡Por lo que se lleva un código de descuento de un 15% de UN SOLO USO para su próxima compra en las tiendas de HealthyCannab!\n\nCódigo: {codigoPromocion.Codigo}\nFecha de caducidad: {codigoPromocion.FechaExpiracion:dd/MM/yyyy}";
                mensaje = mensaje + "\n\n" + mensajeCodigo;
            }
            else if (cantidadTotal >= 5 && !string.IsNullOrEmpty(_carrito.Codigo))
            {
                mensaje += $"\n\nUsted ha realizado una compra de {cantidadTotal} productos en total.\nSin embargo, las políticas de la empresa indican que no puede recibir un código de descuento si está usando uno en esta compra :c";
            }

            await _context.SaveChangesAsync();

            mensaje += "\n\n¡Gracias por elegir a HealthyCannab!";

            // Envia correo
            await _emailSendService.SendEmailAsync("Compra en HealthyCannab", mensaje, usuario.Correo);

            _carrito.Items.Clear();
            _carrito.Descuento = 0;
            _carrito.TotalDescuento = 0;
            _carrito.Codigo = string.Empty;
            ModelState.Clear();

            return View("Success", executedPayment);
        }


        public IActionResult Cancel()
        {
            return View();
        }
    }
}


