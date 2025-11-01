using Firmeza.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.Infrastructure.Persistence;

// DbContext for interacting with the database
public class ApplicationDbContext : DbContext
{
    // Constructor receives options via Dependency Injection
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    // Tables in the database
    public DbSet<Product> Products { get; set; } = null!;
    //public DbSet<Client> Clients { get; set; } = null!;
    //public DbSet<Sale> Sales { get; set; } = null!;
    //public DbSet<SaleDetail> SaleDetails { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // // Configure composite key for SaleDetail
        // modelBuilder.Entity<SaleDetail>()
        //     .HasKey(sd => new { sd.SaleId, sd.ProductId });
        //
        // // Configure relationship: Sale -> Client
        // modelBuilder.Entity<Sale>()
        //     .HasOne(s => s.Client)
        //     .WithMany(c => c.Sales)
        //     .HasForeignKey(s => s.ClientId);
    }
}
