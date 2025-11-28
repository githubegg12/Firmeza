using Firmeza.Domain.Entities;

namespace Firmeza.Domain.Interfaces
{
    /// <summary>
    /// Defines the contract for sale data access operations.
    /// This interface follows the Repository pattern and includes methods for managing sales transactions.
    /// All methods are asynchronous for better performance.
    /// </summary>
    public interface ISaleRepository
    {
        /// <summary>
        /// Retrieves all sales with related entities (User, SaleDetails, Products) asynchronously.
        /// </summary>
        /// <returns>A collection of all sales with their details.</returns>
        Task<IEnumerable<Sale>> GetAllAsync();
        
        /// <summary>
        /// Retrieves a specific sale by its unique identifier, including related entities.
        /// </summary>
        /// <param name="id">The sale ID to search for.</param>
        /// <returns>The sale if found; otherwise, null.</returns>
        Task<Sale?> GetByIdAsync(int id);
        
        /// <summary>
        /// Adds a new sale transaction to the database.
        /// </summary>
        /// <param name="sale">The sale entity to add, including sale details.</param>
        Task AddAsync(Sale sale);
        
        /// <summary>
        /// Updates an existing sale in the database.
        /// </summary>
        /// <param name="sale">The sale entity with updated values.</param>
        Task UpdateAsync(Sale sale);
        
        /// <summary>
        /// Deletes a sale from the database.
        /// </summary>
        /// <param name="sale">The sale entity to delete.</param>
        Task DeleteAsync(Sale sale);
    }
}