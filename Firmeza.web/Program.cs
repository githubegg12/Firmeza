using Firmeza.Application.Interfaces;
using Firmeza.Infrastructure;
using Firmeza.Application.DTOs;

using QuestPDF.Infrastructure;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

// Configure QuestPDF License
QuestPDF.Settings.License = LicenseType.Community;

// FORCE LOGOUT ON RESTART: Use ephemeral keys so cookies from previous runs are invalid.
// This ensures that if the server restarts, all user sessions are invalidated.
builder.Services.AddDataProtection()
    .UseEphemeralDataProtectionProvider();

// --- Service Registration ---
// Bind EmailSettings from configuration
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// Register Infrastructure services (DbContext, Repositories, EmailService, etc.)
builder.Services.AddInfrastructure(builder.Configuration); 

// Register MVC services
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Configure Application Cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

var app = builder.Build();

// --- HTTP Request Pipeline Configuration ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// --- Database Initialization ---
// This now correctly calls our single, authoritative DbInitializer to seed data
await SeedDatabaseAsync(app);

app.Run();

// --- Helper Method for Seeding ---
async Task SeedDatabaseAsync(IHost host)
{
    using (var scope = host.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var dbInitializer = services.GetRequiredService<IDbInitializer>();
            await dbInitializer.InitializeAsync();
        }
        catch (Exception ex)
        {
            // This logger will work because it's being requested from the fully built host
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred during database initialization.");
        }
    }
}
