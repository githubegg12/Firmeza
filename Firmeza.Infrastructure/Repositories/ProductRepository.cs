using Firmeza.Domain.Entities;
using Firmeza.Domain.Interfaces;
using Firmeza.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.Infrastructure.Repositories;
    /// <summary>
    /// Concrete repository implementation for managing Product entities.
    /// Provides CRUD operations using Entity Framework Core.
    /// </summary>
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all products from the database (read-only).
        /// </summary>
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Finds a product by its unique identifier.
        /// </summary>
        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        /// <summary>
        /// Adds a new product to the database.
        /// </summary>
        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            // El guardado se delega al Unit of Work
        }

        /// <summary>
        /// Updates an existing product.
        /// </summary>
        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await Task.CompletedTask; // Se mantiene async, pero no guarda
        }

        /// <summary>
        /// Deletes a product from the database.
        /// </summary>
        public async Task DeleteAsync(Product product)
        {
            _context.Products.Remove(product);
            await Task.CompletedTask; // Se mantiene async, pero no guarda
        }
    }
