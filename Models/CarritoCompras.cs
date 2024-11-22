using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace healthycannab.Models
{
    public class CarritoCompras
    {
        public List<ItemCarrito> Items { get; set; } = new List<ItemCarrito>();
        public decimal Descuento { get; set; } = 0;
        public decimal TotalDescuento { get; set; } = 0;
        public string? Codigo { get; set; }
        public decimal Total => Items.Sum(i => i.Producto.Precio * i.Cantidad);


    }
}