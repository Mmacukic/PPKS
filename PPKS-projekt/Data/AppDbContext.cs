using Microsoft.EntityFrameworkCore;
using PPKS_projekt.Models;

namespace PPKS_projekt.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Shipment> Shipments { get; set; }
    public DbSet<ShipmentStatusLog> ShipmentStatusLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new ShipmentConfiguration());
        modelBuilder.ApplyConfiguration(new ShipmentStatusLogConfiguration());
    }
}