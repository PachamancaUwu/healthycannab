using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace healthycannab.Models
{
    [Table("comentarios")]
    public class Comentario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int ProductoId { get; set; }  // Relación con el producto

        [Required]
        [StringLength(500)]
        public string Contenido { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        public string Usuario { get; set; }  // Nombre del usuario que comenta

        public Comentario()
        {
            Fecha = DateTime.UtcNow;
        }
    }
}