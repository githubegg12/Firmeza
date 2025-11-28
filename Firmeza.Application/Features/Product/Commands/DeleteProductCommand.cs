using Firmeza.Domain.Interfaces;
using Firmeza.Application.Features.Product.Interfaces;

namespace Firmeza.Application.Features.Product.Commands;

/// <summary>
/// Implementation of delete product command
/// </summary>
public class DeleteProductCommand : IDeleteProductCommand
{
    private readonly IProductRepository _repository;

    public DeleteProductCommand(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task ExecuteAsync(int id)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product == null)
            throw new KeyNotFoundException($"Product with ID {id} not found");

        await _repository.DeleteAsync(product);
    }
}

