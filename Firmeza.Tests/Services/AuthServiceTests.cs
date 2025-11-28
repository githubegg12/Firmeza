using Firmeza.Application.DTOs;
using Firmeza.Domain.Entities;
using Firmeza.Identity.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace Firmeza.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
    private readonly Mock<SignInManager<ApplicationUser>> _mockSignInManager;
    private readonly Mock<RoleManager<IdentityRole>> _mockRoleManager;
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        var userStore = new Mock<IUserStore<ApplicationUser>>();
        _mockUserManager = new Mock<UserManager<ApplicationUser>>(userStore.Object, null, null, null, null, null, null, null, null);

        var contextAccessor = new Mock<IHttpContextAccessor>();
        var userPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
        _mockSignInManager = new Mock<SignInManager<ApplicationUser>>(_mockUserManager.Object, contextAccessor.Object, userPrincipalFactory.Object, null, null, null, null);

        var roleStore = new Mock<IRoleStore<IdentityRole>>();
        _mockRoleManager = new Mock<RoleManager<IdentityRole>>(roleStore.Object, null, null, null, null);

        _service = new AuthService(_mockUserManager.Object, _mockSignInManager.Object, _mockRoleManager.Object);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnSuccess_WhenRegistrationIsValid()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "test@test.com",
            Password = "Password123!",
            FirstName = "John",
            LastName = "Doe",
            DocumentId = "123456789",
            PhoneNumber = "1234567890",
            Address = "Test Address"
        };

        _mockUserManager.Setup(m => m.FindByEmailAsync(request.Email))
            .ReturnsAsync((ApplicationUser?)null);

        // Mock Users property for DocumentId check
        var users = new List<ApplicationUser>().AsQueryable();
        _mockUserManager.Setup(m => m.Users).Returns(users);

        _mockUserManager.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), request.Password))
            .ReturnsAsync(IdentityResult.Success);

        _mockRoleManager.Setup(m => m.RoleExistsAsync("Cliente"))
            .ReturnsAsync(true);

        _mockUserManager.Setup(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), "Cliente"))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _service.RegisterAsync(request);

        // Assert
        Assert.True(result.Success);
        _mockUserManager.Verify(m => m.CreateAsync(It.Is<ApplicationUser>(u => 
            u.Email == request.Email && 
            u.DocumentId == request.DocumentId
        ), request.Password), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnError_WhenEmailExists()
    {
        // Arrange
        var request = new RegisterRequest { Email = "existing@test.com" };
        var existingUser = new ApplicationUser { Email = request.Email };

        _mockUserManager.Setup(m => m.FindByEmailAsync(request.Email))
            .ReturnsAsync(existingUser);

        // Act
        var result = await _service.RegisterAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("El email ya está en uso", result.Errors);
    }

    [Fact]
    public async Task SignInAsync_ShouldReturnSuccess_WhenCredentialsAreValid()
    {
        // Arrange
        var email = "test@test.com";
        var password = "Password123!";
        var user = new ApplicationUser { Email = email };

        _mockUserManager.Setup(m => m.FindByEmailAsync(email))
            .ReturnsAsync(user);

        _mockSignInManager.Setup(m => m.PasswordSignInAsync(user, password, false, false))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

        // Act
        var result = await _service.SignInAsync(email, password, false);

        // Assert
        Assert.True(result.Success);
    }

    [Fact]
    public async Task SignInAsync_ShouldReturnError_WhenUserNotFound()
    {
        // Arrange
        var email = "unknown@test.com";
        var password = "Password123!";

        _mockUserManager.Setup(m => m.FindByEmailAsync(email))
            .ReturnsAsync((ApplicationUser?)null);

        // Act
        var result = await _service.SignInAsync(email, password, false);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Email o contraseña inválidos", result.Errors);
    }
}
