using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace healthycannab.Models
{
    public class ProductoDetalleViewModel
    {
        public Producto Producto { get; set; }
        public List<Comentario> Comentarios { get; set; }
        public Comentario NuevoComentario { get; set; }
    }
}