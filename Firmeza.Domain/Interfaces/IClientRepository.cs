using Firmeza.Domain.Entities;

namespace Firmeza.Domain.Interfaces
{
    /// <summary>
    /// Interface for Client repository to abstract data access.
    /// Uses asynchronous methods.
    /// </summary>
    public interface IClientRepository
    {
        Task<IEnumerable<Client>> GetAllAsync();
        Task<Client?> GetByIdAsync(int id);
        Task AddAsync(Client client);
        Task UpdateAsync(Client client);
        Task DeleteAsync(Client client);
    }
}