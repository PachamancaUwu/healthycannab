using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace healthycannab.Models
{
    public class ItemCarrito
    {
        public int ProductoId { get; set; }
        public Producto Producto { get; set; }
        public int Cantidad { get; set; }
    }
}