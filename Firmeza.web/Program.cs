using Firmeza.Application.Interfaces; // Needed for IDbInitializer
using Firmeza.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Service Registration ---

// builder.Services.AddApplication(); // Not needed yet.

// This line is KEY: It registers DbContext, Identity, Repositories, and our DbInitializer.
builder.Services.AddInfrastructure(builder.Configuration); 

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

var app = builder.Build();

// --- 2. HTTP Request Pipeline Configuration ---

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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

// --- 3. Database Initialization ---
await SeedDatabaseAsync(app);

// --- 4. Run the Application ---
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
            // CORRECTED: Calling the method with its proper async name.
            await dbInitializer.InitializeAsync();
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred during database initialization.");
        }
    }
}
