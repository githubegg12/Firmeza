namespace Firmeza.Application.Interfaces;

// Interface for deleting a product
public interface IDeleteProductService
{
    // Delete a product by its Id, returns true if deleted
    Task<bool> DeleteProductAsync(int id);
}