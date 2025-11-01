using Firmeza.Domain.Entities;

namespace Firmeza.Domain.Interfaces;

// Interface for product repository
public interface IProductRepository
{
    IEnumerable<Product> GetAll();
    Product? GetById(int id);
    void Add(Product product);
    void Update(Product product);
    void Delete(Product product);
}
