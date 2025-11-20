using Firmeza.Application.Interfaces;
using Firmeza.Infrastructure;

using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Configure QuestPDF License
QuestPDF.Settings.License = LicenseType.Community;

// --- Service Registration ---
builder.Services.AddInfrastructure(builder.Configuration); 
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

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
// This now correctly calls our single, authoritative DbInitializer
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
