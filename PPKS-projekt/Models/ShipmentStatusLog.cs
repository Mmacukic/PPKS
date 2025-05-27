using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PPKS_projekt.Models;

public class ShipmentStatusLog
{
    public int Id { get; set; }
    public int ShipmentId { get; set; }
    public string Status { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.Now;

    public Shipment Shipment { get; set; }
}

public class ShipmentStatusLogConfiguration : IEntityTypeConfiguration<ShipmentStatusLog>
{
    public void Configure(EntityTypeBuilder<ShipmentStatusLog> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.ChangedAt);

        builder.HasOne(s => s.Shipment)
            .WithMany(sh => sh.StatusLogs)
            .HasForeignKey(s => s.ShipmentId);
    }
}