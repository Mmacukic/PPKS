using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using PPKS_projekt.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using PPKS_projekt.Hubs;
using PPKS_projekt.Models;
using PPKS_projekt.Services;

namespace PPKS_projekt.Controllers
{
    [Authorize]
    public class ShipmentsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<OrderHub> _hubContext;
        private readonly MockShipmentStatusGenerator _generator;



        public ShipmentsController(AppDbContext context, IHubContext<OrderHub> hubContext, MockShipmentStatusGenerator generator)
        {
            _context = context;
            _hubContext = hubContext;
            _generator = generator;
        }
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string newStatus)
        {
            var shipment = await _context.Shipments
                .Include(s => s.StatusLogs)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (shipment == null) return NotFound();

            shipment.CurrentStatus = newStatus;
            shipment.StatusLogs.Add(new ShipmentStatusLog
            {
                ShipmentId = shipment.Id,
                Status = newStatus,
                ChangedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            await _hubContext.Clients.Group($"shipment-{shipment.Id}")
                .SendAsync("ReceiveStatusUpdate", newStatus, DateTime.UtcNow.ToString("g"));

            return Ok();
        }

        public async Task<IActionResult> Index()
        {
            var auth0Id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users
                .Include(u => u.Shipments)
                .FirstOrDefaultAsync(u => u.Auth0Id == auth0Id);

            if (user == null) return Unauthorized();

            return View(user.Shipments.ToList());
        }

        public async Task<IActionResult> Details(int id)
        {
            var shipment = await _context.Shipments
                .Include(s => s.StatusLogs)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (shipment == null) return NotFound();

            _ = Task.Run(() => _generator.SimulateProgressionAsync(id));

            return View(shipment);
        }
    }
}