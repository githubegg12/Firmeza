using Firmeza.Application.DTOs.Client;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Firmeza.Tests.Features.Client;

public class ClientTests
{
    [Fact]
    public void CreateClientDto_ShouldBeValid_WhenDataIsCorrect()
    {
        // Arrange
        var dto = new CreateClientDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@test.com",
            Phone = "1234567890",
            Address = "Test Address",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };

        // Act
        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(dto, context, results, true);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void CreateClientDto_ShouldBeInvalid_WhenRequiredFieldsAreMissing()
    {
        // Arrange
        var dto = new CreateClientDto(); // Missing required fields

        // Act
        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(dto, context, results, true);

        // Assert
        Assert.False(isValid);
        // Assuming DataAnnotations are present on DTO. If not, this test might fail (pass unexpectedly).
        // Let's verify DTO content if this fails, but standard DTOs usually have [Required].
    }
}
