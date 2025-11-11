using Firmeza.Domain.Entities;
using Firmeza.Domain.Interfaces;
using Firmeza.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.Infrastructure.Repositories
{
    /// <summary>
    /// Concrete repository for Sale entity.
    /// Handles database operations asynchronously using EF Core.
    /// Includes related Client and Product data.
    /// </summary>
    public class SaleRepository : ISaleRepository
    {
        private readonly ApplicationDbContext _context;

        public SaleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Sale>> GetAllAsync()
        {
            return await _context.Sales
                .Include(s => s.Client)
                .Include(s => s.SaleDetails)
                .ThenInclude(d => d.Product)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Sale?> GetByIdAsync(int id)
        {
            return await _context.Sales
                .Include(s => s.Client)
                .Include(s => s.SaleDetails)
                .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task AddAsync(Sale sale)
        {
            await _context.Sales.AddAsync(sale);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Sale sale)
        {
            _context.Sales.Update(sale);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Sale sale)
        {
            _context.Sales.Remove(sale);
            await _context.SaveChangesAsync();
        }
    }
}