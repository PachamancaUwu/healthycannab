using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace healthycannab.Models
{
    [Table("detalle_precio")]
    public class DetallePrecio
    {
    
    // Claves Primarias Compuestas
    public int PedidoId { get; set; }
    public Pedido Pedido { get; set; }

    public int ProductoId { get; set; }
    public Producto Producto { get; set; }

    // Atributos
    public int Cantidad { get; set; }

    }
}