using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace healthycannab.Models
{
    [Table("producto")]
    public class Producto
    {
        [Key]  
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
 
        public string? Nombre { get; set; }

        
        public string? Descripcion { get; set; }

        
        [Column(TypeName = "decimal(18,2)")]  
        public decimal Precio { get; set; }

        
        public string? ImagenUrl { get; set; }

        // Relaciones
        public ICollection<Comentario> Comentarios { get; set; }
        public ICollection<DetallePrecio> DetallesPrecios { get; set; }
    }
}
