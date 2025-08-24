using famrhouserent.Models;
using famrhouserent.Services;
using famrhouserent.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace famrhouserent.Controllers
{
    public class VerificationController : Controller
    {
        private readonly EmailService _emailService;
        private readonly ApplicationDbContext _context;

        public VerificationController(EmailService emailService, ApplicationDbContext context)
        {
            _emailService = emailService;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendVerification(string email)
        {
            var otp = Random.Shared.Next(100000, 999999).ToString();
            var user = await _context.UserAccounts.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                // Create temporary password that will be changed after verification
                var tempPassword = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
                user = new User
                {
                    Email = email,
                    IsVerified = false,
                    Otp = otp,
                    OtpExpiry = DateTime.UtcNow.AddMinutes(10),
                    Password = PasswordHasher.HashPassword(tempPassword) // Add temporary password
                };
                _context.UserAccounts.Add(user);
            }
            else
            {
                user.Otp = otp;
                user.OtpExpiry = DateTime.UtcNow.AddMinutes(10);
                user.IsVerified = false;
            }

            await _context.SaveChangesAsync();
            await _emailService.SendOtpEmail(email, otp);

            TempData["Email"] = email;
            return RedirectToAction("VerifyOtp");
        }

        public IActionResult VerifyOtp()
        {
            var email = TempData["Email"]?.ToString();
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Index");
            }
            ViewBag.Email = email;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOtp(string email, string otp)
        {
            var user = await _context.UserAccounts.FirstOrDefaultAsync(u => u.Email == email);
            
            if (user != null && user.Otp == otp && DateTime.UtcNow <= user.OtpExpiry)
            {
                user.IsVerified = true;
                await _context.SaveChangesAsync();
                return View("VerificationSuccess");
            }

            return View("VerificationFailed");
        }
    }
}
