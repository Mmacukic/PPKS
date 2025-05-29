using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PPKS_projekt.Data;
using PPKS_projekt.Hubs;
using PPKS_projekt.Models;

namespace PPKS_projekt.Services
{
    public class MockShipmentStatusGenerator
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHubContext<OrderHub> _hubContext;

        public MockShipmentStatusGenerator(IServiceScopeFactory scopeFactory, IHubContext<OrderHub> hubContext)
        {
            _scopeFactory = scopeFactory;
            _hubContext = hubContext;
        }

        public async Task SimulateProgressionAsync(int shipmentId, CancellationToken cancellationToken = default)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var shipment = await db.Shipments
                .Include(s => s.StatusLogs)
                .FirstOrDefaultAsync(s => s.Id == shipmentId, cancellationToken);

            if (shipment is null) return;

            db.ShipmentStatusLogs.RemoveRange(shipment.StatusLogs);
            shipment.StatusLogs.Clear();

            var orderedStatuses = new[] { "U pripremi", "Na putu", "Isporučeno" };

            foreach (var status in orderedStatuses)
            {
                if (cancellationToken.IsCancellationRequested) break;

                shipment.CurrentStatus = status;
                var log = new ShipmentStatusLog
                {
                    ShipmentId = shipment.Id,
                    Status = status,
                    ChangedAt = DateTime.UtcNow
                };
                shipment.StatusLogs.Add(log);

                await db.SaveChangesAsync(cancellationToken);

                await _hubContext.Clients.Group($"shipment-{shipment.Id}")
                    .SendAsync("ReceiveStatusUpdate", status, log.ChangedAt.ToString("g"), cancellationToken: cancellationToken);

                if (status != "Isporučeno")
                    await Task.Delay(10000, cancellationToken);
            }
        }
    }
}
