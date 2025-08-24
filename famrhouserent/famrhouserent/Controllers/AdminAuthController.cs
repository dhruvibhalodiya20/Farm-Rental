using famrhouserent.Data;
using famrhouserent.Models;
using famrhouserent.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace famrhouserent.Controllers
{
    public class AdminAuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminAuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(AdminLoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var admin = await _context.tblAdmin.FirstOrDefaultAsync(a => a.Email == model.Email);
            if (admin == null || !BCrypt.Net.BCrypt.Verify(model.Password, admin.Password))
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, admin.FullName),
                new Claim(ClaimTypes.Email, admin.Email),
                new Claim("AdminId", admin.AdminId.ToString()),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            TempData["SuccessMessage"] = "Welcome, Admin!";
            return RedirectToAction("Index", "Admin");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
