using Firmeza.Application.DTOs;
using Firmeza.Application.Features.Product.Commands;
using Firmeza.Domain.Interfaces;
using Moq;
using Xunit;

namespace Firmeza.Tests.Features.Product.Commands;

public class UpdateProductCommandTests
{
    private readonly Mock<IProductRepository> _mockRepository;
    private readonly UpdateProductCommand _command;

    public UpdateProductCommandTests()
    {
        _mockRepository = new Mock<IProductRepository>();
        _command = new UpdateProductCommand(_mockRepository.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldUpdateProduct_WhenProductExists()
    {
        // Arrange
        var productId = 1;
        var existingProduct = new Firmeza.Domain.Entities.Product
        {
            Id = productId,
            Name = "Old Name",
            Price = 100,
            Stock = 10
        };

        var dto = new UpdateProductDto
        {
            Name = "New Name",
            Price = 150
        };

        _mockRepository.Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync(existingProduct);

        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Firmeza.Domain.Entities.Product>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _command.ExecuteAsync(productId, dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.Name, result.Name);
        Assert.Equal(dto.Price, result.Price);
        Assert.Equal(existingProduct.Stock, result.Stock); // Should remain unchanged

        _mockRepository.Verify(r => r.UpdateAsync(It.Is<Firmeza.Domain.Entities.Product>(p =>
            p.Name == dto.Name &&
            p.Price == dto.Price &&
            p.Stock == existingProduct.Stock
        )), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowKeyNotFoundException_WhenProductDoesNotExist()
    {
        // Arrange
        var productId = 1;
        var dto = new UpdateProductDto { Name = "New Name" };

        _mockRepository.Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync((Firmeza.Domain.Entities.Product?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _command.ExecuteAsync(productId, dto));
    }
}
