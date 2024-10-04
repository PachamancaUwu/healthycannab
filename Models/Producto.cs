using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace healthycannab.Models
{
    public class Producto
    {
    public int Id { get; set; }
    public string? Nombre { get; set; }
    public string? Descripcion { get; set; }
    public decimal? Precio { get; set; }
    public string? ImagenUrl { get; set; }
    }
}