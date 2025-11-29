using Firmeza.Application.DTOs.Sale;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Interfaces;
using Firmeza.Infrastructure.Services;
using Moq;
using Xunit;

namespace Firmeza.Tests.Services;

/// <summary>
/// Unit tests for SaleService business logic
/// </summary>
public class SaleServiceTests
{
    private readonly Mock<ISaleRepository> _mockRepository;
    private readonly SaleService _service;

    public SaleServiceTests()
    {
        _mockRepository = new Mock<ISaleRepository>();
        _service = new SaleService(_mockRepository.Object);
    }

    /// <summary>
    /// Verifies that CountAsync returns the correct number of sales
    /// </summary>
    [Fact]
    public async Task CountAsync_ShouldReturnCorrectCount()
    {
        // Arrange
        var sales = new List<Sale>
        {
            new() { Id = 1 },
            new() { Id = 2 }
        };

        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(sales);

        // Act
        var result = await _service.CountAsync();

        // Assert
        Assert.Equal(2, result);
    }

    /// <summary>
    /// Verifies that GetTotalRevenueAsync calculates the correct sum
    /// </summary>
    [Fact]
    public async Task GetTotalRevenueAsync_ShouldReturnSumOfTotalAmounts()
    {
        // Arrange
        var sales = new List<Sale>
        {
            new() { Id = 1, TotalAmount = 100 },
            new() { Id = 2, TotalAmount = 200 }
        };

        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(sales);

        // Act
        var result = await _service.GetTotalRevenueAsync();

        // Assert
        Assert.Equal(300, result);
    }

    /// <summary>
    /// Verifies that GetProductRevenueReportAsync groups and aggregates data correctly
    /// </summary>
    [Fact]
    public async Task GetProductRevenueReportAsync_ShouldReturnGroupedReport()
    {
        // Arrange
        var product1 = new Firmeza.Domain.Entities.Product { Id = 1, Name = "P1", Price = 10 };
        var product2 = new Firmeza.Domain.Entities.Product { Id = 2, Name = "P2", Price = 20 };

        var sales = new List<Sale>
        {
            new()
            {
                Id = 1,
                SaleDetails = new List<SaleDetail>
                {
                    new() { ProductId = 1, Product = product1, Quantity = 2, UnitPrice = 10 },
                    new() { ProductId = 2, Product = product2, Quantity = 1, UnitPrice = 20 }
                }
            },
            new()
            {
                Id = 2,
                SaleDetails = new List<SaleDetail>
                {
                    new() { ProductId = 1, Product = product1, Quantity = 3, UnitPrice = 10 }
                }
            }
        };

        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(sales);

        // Act
        var result = await _service.GetProductRevenueReportAsync();

        // Assert
        Assert.NotNull(result);
        var report = result.ToList();
        Assert.Equal(2, report.Count);

        var p1Report = report.FirstOrDefault(r => r.ProductId == 1);
        Assert.NotNull(p1Report);
        Assert.Equal("P1", p1Report.ProductName);
        Assert.Equal(5, p1Report.TotalQuantitySold); // 2 + 3
        Assert.Equal(50, p1Report.TotalRevenue); // 20 + 30

        var p2Report = report.FirstOrDefault(r => r.ProductId == 2);
        Assert.NotNull(p2Report);
        Assert.Equal("P2", p2Report.ProductName);
        Assert.Equal(1, p2Report.TotalQuantitySold);
        Assert.Equal(20, p2Report.TotalRevenue);
    }
}
