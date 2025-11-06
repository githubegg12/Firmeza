using Firmeza.Application.DTOs;

namespace Firmeza.Application.Interfaces;

// Service interface for product operations
public interface IProductService
{
    Task<IEnumerable<ProductDto>>GetAllProducts();
    Task <ProductDto?> GetProductById(int id);
}
