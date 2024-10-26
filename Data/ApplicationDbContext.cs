using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace healthycannab.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    public DbSet<healthycannab.Models.Usuario> DataUsuario { get; set;}
    public DbSet<healthycannab.Models.Contacto> DataContacto { get; set;}

    public DbSet<healthycannab.Models.Producto> DataProducto { get; set; }

     protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<healthycannab.Models.Producto>().HasData(
            new healthycannab.Models.Producto { Id = 1, Nombre = "Producto 1", Descripcion = "Aceite de cannabis 100% puro", Precio = 150, ImagenUrl = "/img/a.png" },
            new healthycannab.Models.Producto { Id = 2, Nombre = "Producto 2", Descripcion = "Ungüento de cannabis", Precio = 120, ImagenUrl = "/img/b.jpg" },
            new healthycannab.Models.Producto { Id = 3, Nombre = "Producto 3", Descripcion = "Cannabis CBD cannabidiol", Precio = 95, ImagenUrl = "/img/canabis3.png" },
            new healthycannab.Models.Producto { Id = 4, Nombre = "Producto 4", Descripcion = "Cannabis To you", Precio = 105, ImagenUrl = "/img/canabis4.png" },
            new healthycannab.Models.Producto { Id = 5, Nombre = "Producto 5", Descripcion = "Cannabis Gotas", Precio = 99, ImagenUrl = "/img/cannabisGotas.jpg" }
        );
    }

}
