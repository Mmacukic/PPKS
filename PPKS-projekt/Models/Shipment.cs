using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PPKS_projekt.Models;

public class Shipment
{
    public int Id { get; set; }
    public string TrackingNumber { get; set; }
    public string CurrentStatus { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public int UserId { get; set; }
    public User User { get; set; }

    public ICollection<ShipmentStatusLog> StatusLogs { get; set; }
}

public class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
{
    public void Configure(EntityTypeBuilder<Shipment> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.TrackingNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(s => s.TrackingNumber).IsUnique();

        builder.Property(s => s.CurrentStatus)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.CreatedAt);

        builder.HasOne(s => s.User)
            .WithMany(u => u.Shipments)
            .HasForeignKey(s => s.UserId);
    }
}