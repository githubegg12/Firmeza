namespace Firmeza.Application.Features.Product.Interfaces;

/// <summary>
/// Interface for delete product commands
/// </summary>
public interface IDeleteProductCommand
{
    /// <summary>
    /// Executes the deletion of a product by ID
    /// </summary>
    /// <param name="id">Product identifier</param>
    Task ExecuteAsync(int id);
}

