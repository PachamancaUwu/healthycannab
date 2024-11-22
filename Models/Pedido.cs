using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace healthycannab.Models
{
    [Table("pedido")]
    public class Pedido
    {
        [Key]  
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }

        // Claves Foráneas
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        public int? CodigoPromocionId { get; set; }
        public CodigoPromocion? CodigoPromocion { get; set; }

        // Relaciones
        public ICollection<DetallePrecio> DetallesPrecios { get; set; }

    
    }
}