using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace healthycannab.Models
{
    [Table("contacto")]
    public class Contacto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id {   get; set;}
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Celular { get; set; }
        public string Mensaje { get; set; }
        
    }
}