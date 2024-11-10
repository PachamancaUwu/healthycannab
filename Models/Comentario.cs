using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace healthycannab.Models
{
    [Table("comentario")]
    public class Comentario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime Fecha { get; set; }

        public string Contenido { get; set; }

        // Claves Foráneas
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        public int ProductoId { get; set; }
        public Producto Producto { get; set; }
    }
}