using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PPKS_projekt.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace PPKS_projekt.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly AppDbContext _context;

        public ProfileController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var auth0Id = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Auth0Id == auth0Id);

            if (user == null)
                return NotFound("Korisnik nije pronađen u bazi.");

            return View(user);
        }
    }
}