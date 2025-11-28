using Firmeza.Domain.Entities;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.Infrastructure.Data
{
    /// <summary>
    /// Main Entity Framework Core database context for the Firmeza application.
    /// Inherits from IdentityDbContext to integrate ASP.NET Core Identity with custom ApplicationUser.
    /// Manages all database entities and their relationships.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        /// <summary>
        /// Initializes a new instance of the ApplicationDbContext class.
        /// </summary>
        /// <param name="options">The options to be used by the DbContext.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        /// <summary>
        /// Gets or sets the DbSet for Product entities.
        /// </summary>
        public DbSet<Product> Products { get; set; }
        
        /// <summary>
        /// Gets or sets the DbSet for Sale entities.
        /// Note: Clients table was removed - using AspNetUsers (ApplicationUser) instead.
        /// </summary>
        public DbSet<Sale> Sales { get; set; }
        
        /// <summary>
        /// Gets or sets the DbSet for SaleDetail entities.
        /// </summary>
        public DbSet<SaleDetail> SaleDetails { get; set; }

        /// <summary>
        /// Configures the database schema using Fluent API.
        /// Defines entity relationships and constraints.
        /// </summary>
        /// <param name="builder">The ModelBuilder instance used to configure the database schema.</param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ---- SALE ↔ USER (Identity) RELATIONSHIP ----
            builder.Entity<Sale>()
                .HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ---- SALE DETAILS RELATIONSHIPS ----
            builder.Entity<SaleDetail>()
                .HasOne(sd => sd.Sale)
                .WithMany(s => s.SaleDetails)
                .HasForeignKey(sd => sd.SaleId);

            builder.Entity<SaleDetail>()
                .HasOne(sd => sd.Product)
                .WithMany(p => p.SaleDetails)
                .HasForeignKey(sd => sd.ProductId);
        }
    }
}
