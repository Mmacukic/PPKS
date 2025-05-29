using Microsoft.EntityFrameworkCore;
using PPKS_projekt.Models;

namespace PPKS_projekt.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(AppDbContext context)
        {
            // Provjera postoji li već barem jedan shipment
            if (await context.Shipments.AnyAsync())
                return;

            // Pronađi jednog postojećeg korisnika
            var user = await context.Users.FirstOrDefaultAsync();
            if (user == null)
                return; // ne možeš kreirati shipment bez korisnika

            var shipments = new List<Shipment>
            {
                new Shipment
                {
                    TrackingNumber = "HR123456789",
                    CurrentStatus = "U pripremi",
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    UserId = user.Id
                },
                new Shipment
                {
                    TrackingNumber = "HR987654321",
                    CurrentStatus = "U pripremi",
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    UserId = user.Id
                },
                new Shipment
                {
                    TrackingNumber = "HR555888777",
                    CurrentStatus = "U pripremi",
                    CreatedAt = DateTime.UtcNow.AddHours(-6),
                    UserId = user.Id
                }
            };

            context.Shipments.AddRange(shipments);
            await context.SaveChangesAsync();
        }
    }
}