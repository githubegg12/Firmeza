using Firmeza.Application.DTOs;
using Firmeza.Application.Interfaces;
using Firmeza.Application.MappingProfiles;
using Firmeza.Identity.DTOs;
using Firmeza.Identity.Services;
using Firmeza.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// -------------------------
// Configure Services
// -------------------------

// Add Infrastructure services (DbContext, Identity, Repositories, etc.)
builder.Services.AddInfrastructure(builder.Configuration);

// Configure JWT and Email Settings from appsettings.json
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// Add JWT Token Service for generating authentication tokens
builder.Services.AddScoped<JwtTokenService>();

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
var key = Encoding.UTF8.GetBytes(jwtSettings?.SecretKey ?? throw new InvalidOperationException("JWT SecretKey not configured"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero // No tolerance for token expiration
    };
});

// Add Authorization services
builder.Services.AddAuthorization();

// Add AutoMapper with mapping profiles
builder.Services.AddAutoMapper(typeof(ProductProfile), typeof(SaleProfile));

// Add Controllers
builder.Services.AddControllers();

// Configure CORS (Cross-Origin Resource Sharing)
builder.Services.AddCors(options =>
{
    // Primary CORS policy for Angular client
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Angular default port
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // Required for authentication cookies/tokens
        
        // Note: Cannot use AllowAnyOrigin with AllowCredentials
    });
    
    // Fallback CORS policy for development/testing
    options.AddPolicy("DevCors", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configure Swagger/OpenAPI for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Firmeza API",
        Version = "v1",
        Description = "RESTful API para el sistema Firmeza - Gestión de productos, clientes y ventas",
        Contact = new OpenApiContact
        {
            Name = "Firmeza Team",
            Email = "contact@firmeza.com"
        }
    });

    // Configure JWT Authentication in Swagger UI
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese 'Bearer' seguido de un espacio y el token JWT.\\n\\nEjemplo: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Include XML comments in Swagger if available
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// -------------------------
// Configure HTTP Request Pipeline
// -------------------------

// Enable Swagger in all environments for testing and documentation
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Firmeza API v1");
    options.RoutePrefix = string.Empty; // Swagger UI at root URL
});

// Redirect HTTP to HTTPS
app.UseHttpsRedirection();

// Apply CORS policy
app.UseCors("AllowAll");

// Enable authentication middleware
app.UseAuthentication();

// Enable authorization middleware
app.UseAuthorization();

// Map controller endpoints
app.MapControllers();

// -------------------------
// Database Initialization
// -------------------------
// Seed database with initial data (roles, admin user, test data)
await SeedDatabaseAsync(app);

app.Run();

// Helper method for database seeding
async Task SeedDatabaseAsync(IHost host)
{
    using var scope = host.Services.CreateScope();
    var services = scope.ServiceProvider;
    try
    {
        var dbInitializer = services.GetRequiredService<IDbInitializer>();
        await dbInitializer.InitializeAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during database initialization.");
    }
}