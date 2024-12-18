﻿using healthycannab.Models;
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
    public DbSet<healthycannab.Models.Comentario>  DataComentario { get; set;}

    
    public DbSet<healthycannab.Models.Pedido> DataPedido { get; set;}
    public DbSet<healthycannab.Models.DetallePrecio> DataDetallePrecio { get; set;}
    public DbSet<healthycannab.Models.CodigoPromocion> DataCodigoPromocion { get; set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de DetallePrecio para llaves compuestas
        modelBuilder.Entity<healthycannab.Models.DetallePrecio>()
            .HasKey(dp => new { dp.PedidoId, dp.ProductoId });

        // Configurar relación entre Comentario y Usuario
        modelBuilder.Entity<Comentario>()
            .HasOne(c => c.Usuario)
            .WithMany(u => u.Comentarios)
            .HasForeignKey(c => c.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configurar relación entre Comentario y Producto
        modelBuilder.Entity<Comentario>()
            .HasOne(c => c.Producto)
            .WithMany(p => p.Comentarios)
            .HasForeignKey(c => c.ProductoId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relaciones de Usuario y Pedido
        modelBuilder.Entity<healthycannab.Models.Pedido>()
            .HasOne(p => p.Usuario)
            .WithMany(u => u.Pedidos)
            .HasForeignKey(p => p.UsuarioId);
                
        //Relación One to One de CodigoPromocion y Pedido
        modelBuilder.Entity<healthycannab.Models.Pedido>()
            .HasOne(p => p.CodigoPromocion)
            .WithOne(cp => cp.Pedido)
            .HasForeignKey<Pedido>(p => p.CodigoPromocionId)
            .OnDelete(DeleteBehavior.SetNull); // Si se elimina un código de promoción, no elimina los pedidos

        // Relaciones de Pedido y DetallePrecio
        modelBuilder.Entity<healthycannab.Models.DetallePrecio>()
            .HasOne(dp => dp.Pedido)
            .WithMany(p => p.DetallesPrecios)
            .HasForeignKey(dp => dp.PedidoId);

        // Relaciones de Producto y DetallePrecio
        modelBuilder.Entity<healthycannab.Models.DetallePrecio>()
            .HasOne(dp => dp.Producto)
            .WithMany(pr => pr.DetallesPrecios)
            .HasForeignKey(dp => dp.ProductoId);

        // Restricción de unicidad en CodigoPromocion
        modelBuilder.Entity<healthycannab.Models.CodigoPromocion>()
            .HasIndex(cp => cp.Codigo)
            .IsUnique();
    }

}
