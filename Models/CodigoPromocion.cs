using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace healthycannab.Models
{
    [Table("codigo_promocion")]
    public class CodigoPromocion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id {   get; set;}
        public string Codigo { get; set; }
        public decimal Descuento { get; set; }
        public bool Usado { get; set; } = false;
        public DateTime FechaExpiracion { get; set; }

        //Clase para crear string de código
        public static string GenerarCodigo(int longitud = 12)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return $"HEALTHYCANNAB-{new string(Enumerable.Repeat(chars, longitud)
                .Select(s => s[random.Next(s.Length)]).ToArray())}";
        }
        
        // Relaciones
        public Pedido? Pedido { get; set; }
    }
}