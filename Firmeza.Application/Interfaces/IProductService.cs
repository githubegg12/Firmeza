using Firmeza.Application.DTOs;
using System.Collections.Generic;

namespace Firmeza.Application.Interfaces;

// Service interface for product operations
public interface IProductService
{
    IEnumerable<ProductDto> GetAllProducts();
    ProductDto? GetProductById(int id);
}
