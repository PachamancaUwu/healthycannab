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
    public DbSet<healthycannab.Models.Producto> DataProducto { get; set;}

}
