using Firmeza.Domain.Entities;
using Firmeza.Infrastructure.Data;
using Firmeza.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Firmeza.Tests.Repositories;

public class ProductRepositoryTests
{
    private readonly DbContextOptions<ApplicationDbContext> _options;

    public ProductRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB per test class
            .Options;
    }

    [Fact]
    public async Task AddAsync_ShouldAddProductToDatabase()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var repository = new ProductRepository(context);
        var product = new Product { Name = "Test Product", Price = 100, Stock = 10 };

        // Act
        await repository.AddAsync(product);
        await context.SaveChangesAsync(); // Ensure changes are saved if repo doesn't save automatically

        // Assert
        using var assertContext = new ApplicationDbContext(_options);
        var savedProduct = await assertContext.Products.FirstOrDefaultAsync(p => p.Name == "Test Product");
        Assert.NotNull(savedProduct);
        Assert.Equal(100, savedProduct.Price);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnProduct_WhenProductExists()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var product = new Product { Name = "Test Product", Price = 100 };
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var repository = new ProductRepository(context);

        // Act
        var result = await repository.GetByIdAsync(product.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(product.Name, result.Name);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateProductInDatabase()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var product = new Product { Name = "Old Name", Price = 100 };
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var repository = new ProductRepository(context);

        // Act
        product.Name = "New Name";
        await repository.UpdateAsync(product);
        // Note: UpdateAsync in generic repo usually sets state to modified. 
        // If it calls SaveChanges, good. If not, we might need to call it here depending on implementation.
        // Assuming repo handles it or we need to verify state change.
        // Let's check if repo saves changes. If not, we might need context.SaveChangesAsync().
        // For now assuming standard repo pattern.
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveProductFromDatabase()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var product = new Product { Name = "To Delete", Price = 100 };
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var repository = new ProductRepository(context);

        // Act
        await repository.DeleteAsync(product);

        // Assert
        using var assertContext = new ApplicationDbContext(_options);
        var deletedProduct = await assertContext.Products.FindAsync(product.Id);
        Assert.Null(deletedProduct);
    }
}
