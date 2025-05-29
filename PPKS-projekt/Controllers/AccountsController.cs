using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using PPKS_projekt.Data;
using PPKS_projekt.Models;

namespace PPKS_projekt.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Login(string returnUrl = "/")
        {
            var redirectUrl = Url.Action("Callback", "Account", new { returnUrl });
            return Challenge(new AuthenticationProperties { RedirectUri = redirectUrl }, "Auth0");
        }

        public IActionResult Logout()
        {
            var callbackUrl = Url.Action("Index", "Home", null, Request.Scheme);

            return SignOut(
                new AuthenticationProperties { RedirectUri = callbackUrl },
                "Auth0",
                CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> Callback(string returnUrl = "/")
        {
            var auth0Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst("name")?.Value;
            var firstName = User.FindFirst("nickname")?.Value;
            var lastName = ""; 

            if (string.IsNullOrEmpty(email))
                return BadRequest("Email nije pronađen među claimovima.");

            var existingUser = _context.Users.FirstOrDefault(u => u.Auth0Id == auth0Id);
            if (existingUser == null)
            {
                var newUser = new User
                {
                    Auth0Id = auth0Id!,
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();
            }

            return LocalRedirect(returnUrl);
        }
    }
}