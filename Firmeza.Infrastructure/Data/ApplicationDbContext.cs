using Firmeza.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.Infrastructure.Data
{
    /// <summary>
    /// Main EF Core database context for the application.
    /// Inherits from IdentityDbContext<User> to connect the custom User entity with ASP.NET Core Identity.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser> // CORRECTED: Using the correct 'User' entity
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleDetail> SaleDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure relationships and constraints explicitly
            builder.Entity<Sale>()
                .HasOne(s => s.Client)
                .WithMany(c => c.Sales)
                .HasForeignKey(s => s.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<SaleDetail>()
                .HasOne(sd => sd.Sale)
                .WithMany(s => s.SaleDetails) // CORRECTED: This should point to the correct navigation property
                .HasForeignKey(sd => sd.SaleId);

            builder.Entity<SaleDetail>()
                .HasOne(sd => sd.Product)
                .WithMany(p => p.SaleDetails)
                .HasForeignKey(sd => sd.ProductId);
        }
    }
}
