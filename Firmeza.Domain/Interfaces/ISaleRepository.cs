using Firmeza.Domain.Entities;

namespace Firmeza.Domain.Interfaces
{
    /// <summary>
    /// Interface for Sale repository to abstract data access.
    /// Uses asynchronous methods.
    /// </summary>
    public interface ISaleRepository
    {
        Task<IEnumerable<Sale>> GetAllAsync();
        Task<Sale?> GetByIdAsync(int id);
        Task AddAsync(Sale sale);
        Task UpdateAsync(Sale sale);
        Task DeleteAsync(Sale sale);
    }
}