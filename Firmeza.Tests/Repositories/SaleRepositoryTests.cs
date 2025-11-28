using Firmeza.Domain.Entities;
using Firmeza.Infrastructure.Data;
using Firmeza.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Firmeza.Tests.Repositories;

public class SaleRepositoryTests
{
    private readonly DbContextOptions<ApplicationDbContext> _options;

    public SaleRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task AddAsync_ShouldAddSaleWithDetails()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var repository = new SaleRepository(context);
        
        var sale = new Sale 
        { 
            SaleDate = DateTime.Now,
            TotalAmount = 300,
            SaleDetails = new List<SaleDetail>
            {
                new() { ProductId = 1, Quantity = 2, UnitPrice = 100 },
                new() { ProductId = 2, Quantity = 1, UnitPrice = 100 }
            }
        };

        // Act
        await repository.AddAsync(sale);
        await context.SaveChangesAsync();

        // Assert
        using var assertContext = new ApplicationDbContext(_options);
        var savedSale = await assertContext.Sales
            .Include(s => s.SaleDetails)
            .FirstOrDefaultAsync(s => s.TotalAmount == 300);

        Assert.NotNull(savedSale);
        Assert.Equal(2, savedSale.SaleDetails.Count);
    }



    [Fact]
    public async Task GetAllAsync_ShouldReturnSalesWithDetails()
    {
        // Arrange
        using (var context = new ApplicationDbContext(_options))
        {
            var user = new ApplicationUser { Id = "user1", UserName = "testuser", Email = "test@test.com" };
            context.Users.Add(user);

            var product = new Product { Id = 1, Name = "Product 1", Price = 100 };
            context.Products.Add(product);

            var sale = new Sale 
            { 
                SaleDate = DateTime.Now,
                TotalAmount = 100,
                UserId = user.Id,
                SaleDetails = new List<SaleDetail>
                {
                    new() { ProductId = 1, Quantity = 1, UnitPrice = 100 }
                }
            };
            context.Sales.Add(sale);
            await context.SaveChangesAsync();
        }

        using var actContext = new ApplicationDbContext(_options);
        var repository = new SaleRepository(actContext);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotEmpty(result);
        var firstSale = result.First();
        Assert.NotNull(firstSale.SaleDetails);
        Assert.NotEmpty(firstSale.SaleDetails);
    }
}
