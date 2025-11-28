using Firmeza.Application.Features.Product.Commands;
using Firmeza.Domain.Interfaces;
using Moq;
using Xunit;

namespace Firmeza.Tests.Features.Product.Commands;

public class DeleteProductCommandTests
{
    private readonly Mock<IProductRepository> _mockRepository;
    private readonly DeleteProductCommand _command;

    public DeleteProductCommandTests()
    {
        _mockRepository = new Mock<IProductRepository>();
        _command = new DeleteProductCommand(_mockRepository.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldDeleteProduct_WhenProductExists()
    {
        // Arrange
        var productId = 1;
        var existingProduct = new Firmeza.Domain.Entities.Product { Id = productId, Name = "Test Product" };

        _mockRepository.Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync(existingProduct);

        _mockRepository.Setup(r => r.DeleteAsync(It.IsAny<Firmeza.Domain.Entities.Product>()))
            .Returns(Task.CompletedTask);

        // Act
        await _command.ExecuteAsync(productId);

        // Assert
        _mockRepository.Verify(r => r.DeleteAsync(existingProduct), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowKeyNotFoundException_WhenProductDoesNotExist()
    {
        // Arrange
        var productId = 1;

        _mockRepository.Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync((Firmeza.Domain.Entities.Product?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _command.ExecuteAsync(productId));
    }
}
