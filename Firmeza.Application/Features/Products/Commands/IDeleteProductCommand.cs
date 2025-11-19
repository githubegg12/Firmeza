namespace Firmeza.Application.Features.Products.Commands;

/// <summary>
/// Interface for delete product commands
/// </summary>
public interface IDeleteProductCommand
{
    Task ExecuteAsync(int id);
}

