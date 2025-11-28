using Firmeza.Application.Features.Product.Queries;
using Firmeza.Domain.Interfaces;
using Moq;
using Xunit;

namespace Firmeza.Tests.Features.Product.Queries;

public class GetProductQueryTests
{
    private readonly Mock<IProductRepository> _mockRepository;
    private readonly GetProductQuery _query;

    public GetProductQueryTests()
    {
        _mockRepository = new Mock<IProductRepository>();
        _query = new GetProductQuery(_mockRepository.Object);
    }

    [Fact]
    public async Task GetAllProductsAsync_ShouldReturnAllProducts_WhenProductsExist()
    {
        // Arrange
        var products = new List<Firmeza.Domain.Entities.Product>
        {
            new() { Id = 1, Name = "Product 1", Price = 100 },
            new() { Id = 2, Name = "Product 2", Price = 200 }
        };

        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(products);

        // Act
        var result = await _query.GetAllProductsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, p => p.Name == "Product 1");
        Assert.Contains(result, p => p.Name == "Product 2");
    }

    [Fact]
    public async Task GetProductByIdAsync_ShouldReturnProduct_WhenProductExists()
    {
        // Arrange
        var productId = 1;
        var product = new Firmeza.Domain.Entities.Product { Id = productId, Name = "Test Product" };

        _mockRepository.Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync(product);

        // Act
        var result = await _query.GetProductByIdAsync(productId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(productId, result.Id);
        Assert.Equal(product.Name, result.Name);
    }

    [Fact]
    public async Task GetProductByIdAsync_ShouldReturnNull_WhenProductDoesNotExist()
    {
        // Arrange
        var productId = 1;

        _mockRepository.Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync((Firmeza.Domain.Entities.Product?)null);

        // Act
        var result = await _query.GetProductByIdAsync(productId);

        // Assert
        Assert.Null(result);
    }
}
