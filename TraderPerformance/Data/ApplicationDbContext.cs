using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TraderPerformance.Models;

namespace TraderPerformance.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Security> Securities { get; set; }
    public DbSet<Trade> Trades { get; set; }
    public DbSet<Portfolio> Portfolios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Trade>()
            .HasOne(t => t.Security)
            .WithMany(s => s.Trades)
            .HasForeignKey(t => t.SecurityID)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Trade>()
            .HasOne(t => t.Portfolio)
            .WithMany(p => p.Trades)
            .HasForeignKey(t => t.PortfolioID)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Portfolio>()
            .HasOne(p => p.User)
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Trade>()
            .HasOne(t => t.User)
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}