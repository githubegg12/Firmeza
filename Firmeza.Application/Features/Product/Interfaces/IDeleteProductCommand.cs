namespace Firmeza.Application.Features.Product.Interfaces;

/// <summary>
/// Interface for delete product commands
/// </summary>
public interface IDeleteProductCommand
{
    Task ExecuteAsync(int id);
}

