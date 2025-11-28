using Firmeza.Application.DTOs;
using Firmeza.Application.Features.Product.Commands;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Interfaces;
using Moq;
using Xunit;

namespace Firmeza.Tests.Features.Product.Commands;

public class CreateProductCommandTests
{
    private readonly Mock<IProductRepository> _mockRepository;
    private readonly CreateProductCommand _command;

    public CreateProductCommandTests()
    {
        _mockRepository = new Mock<IProductRepository>();
        _command = new CreateProductCommand(_mockRepository.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCreateProduct_WhenDtoIsValid()
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = "Test Product",
            Description = "Test Description",
            Category = "Test Category",
            Price = 100,
            Stock = 10,
            ImageUrl = "http://test.com/image.jpg"
        };

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Firmeza.Domain.Entities.Product>()))
            .Callback<Firmeza.Domain.Entities.Product>(p => p.Id = 1) // Simulate DB ID generation
            .Returns(Task.CompletedTask);

        // Act
        var result = await _command.ExecuteAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(dto.Name, result.Name);
        Assert.Equal(dto.Price, result.Price);
        
        _mockRepository.Verify(r => r.AddAsync(It.Is<Firmeza.Domain.Entities.Product>(p => 
            p.Name == dto.Name && 
            p.Price == dto.Price
        )), Times.Once);
    }
}
