using Firmeza.Application.Interfaces;
using Firmeza.Domain.Interfaces;

namespace Firmeza.Application.Services;

// Service for deleting products
public class DeleteProductService : IDeleteProductService
{
    private readonly IProductRepository _repo;

    // Constructor injection of repository
    public DeleteProductService(IProductRepository repo)
    {
        _repo = repo;
    }

    // Delete product by ID
    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _repo.GetByIdAsync(id); // Fetch product
        if (product == null) return false;          // Return false if not found

        await _repo.DeleteAsync(product);           // Remove product from repository
        // Note: SaveChangesAsync() should be called in UnitOfWork or higher layer

        return true; // Successfully deleted
    }
}
