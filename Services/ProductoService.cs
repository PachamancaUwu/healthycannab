using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using healthycannab.Data;
using healthycannab.Models;
using Microsoft.EntityFrameworkCore;

namespace healthycannab.Services
{
    public class ProductoService
    {
        //private readonly ILogger<ProductoService> _logger;
        private readonly ApplicationDbContext _context;

        // public ProductoService(ILogger<ProductoService> logger, ApplicationDbContext context)
        // {
        //     _logger= logger;
        //     _context=context;
        // }
        
        public ProductoService(ApplicationDbContext context)
        {            
            _context=context;
        }
         public async Task<List<Producto>?> GetAll(){
            if(_context.DataProducto == null )
                return null;
            return await _context.DataProducto.ToListAsync();
        }

        public async Task<List<Producto>> FilterProductos(string? nombre, decimal? precioMin, decimal? precioMax)
        {
            var query = _context.DataProducto.AsQueryable();

            if(!string.IsNullOrEmpty(nombre))
            {
                query = query.Where(p => p.Nombre.Contains(nombre));

            }
            if (precioMin.HasValue)
            {
                query = query.Where(p => p.Precio >= precioMin);
            }
            if (precioMax.HasValue)
            {
                query =query.Where(p => p.Precio <=precioMax);
            }

            return await query.ToListAsync();

        }
    }
}