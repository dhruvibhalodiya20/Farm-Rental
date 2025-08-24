using famrhouserent.Models;
using famrhouserent.Models.ViewModels;
using famrhouserent.Services;
using famrhouserent.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using Microsoft.Extensions.Configuration;

namespace famrhouserent.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _configuration;

        public AuthController(
            ApplicationDbContext context, 
            EmailService emailService,
            ILogger<AuthController> logger,
            IConfiguration configuration)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);

                // Check existing user
                if (await _context.UserAccounts.AnyAsync(u => u.Email == model.Email.ToLower()))
                {
                    ModelState.AddModelError("Email", "This email is already registered");
                    return View(model);
                }

                // Create user
                var user = new User
                {
                    Email = model.Email.ToLower(),
                    Password = PasswordHasher.HashPassword(model.Password),
                    Name = model.Name,
                    IsVerified = false
                };

                // Generate OTP
                var otp = Random.Shared.Next(100000, 999999).ToString("D6");
                user.Otp = otp;
                user.OtpExpiry = DateTime.UtcNow.AddMinutes(10);

                _context.UserAccounts.Add(user);
                await _context.SaveChangesAsync();

                // Send OTP email
                bool emailSent = await _emailService.SendOtpAsync(user.Email, otp);
                
                if (!emailSent)
                {
                    _logger.LogError($"Failed to send OTP to {user.Email}");
                    TempData["Warning"] = "Account created but verification email could not be sent. Please use 'Resend OTP' option.";
                }
                else
                {
                    TempData["Success"] = "Please check your email for the verification code.";
                }

                return RedirectToAction("VerifyEmail", new { email = user.Email });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
                ModelState.AddModelError("", "An error occurred during registration. Please try again.");
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ResendOtp(string email)
        {
            try
            {
                var user = await _context.UserAccounts.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not found" });
                }

                var otp = Random.Shared.Next(100000, 999999).ToString();
                user.Otp = otp;
                user.OtpExpiry = DateTime.UtcNow.AddMinutes(10);
                await _context.SaveChangesAsync();

                bool emailSent = await _emailService.SendOtpEmail(user.Email, otp);
                
                if (!emailSent)
                {
                    return Json(new { success = false, message = "Failed to send OTP. Please try again." });
                }

                return Json(new { success = true, message = "OTP sent successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error resending OTP to {email}");
                return Json(new { success = false, message = "An error occurred. Please try again." });
            }
        }

        private bool IsPasswordStrong(string password)
        {
            return password.Length >= 8 &&
                   password.Any(char.IsUpper) &&
                   password.Any(char.IsLower) &&
                   password.Any(char.IsDigit) &&
                   password.Any(ch => !char.IsLetterOrDigit(ch));
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            

            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.UserAccounts.FirstOrDefaultAsync(u => u.Email == model.Email);
            
            if (user == null || !PasswordHasher.VerifyPassword(user.Password, model.Password))
            {
                ModelState.AddModelError("", "Invalid email or password");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name ?? string.Empty),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            if (user.IsAdmin)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            TempData["LoginSuccess"] = "Welcome back!";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var user = await _context.UserAccounts.FirstOrDefaultAsync(u => u.Email == model.Email);
                
                if (user == null)
                {
                    ModelState.AddModelError("", "If an account exists with this email, you will receive an OTP.");
                    return View(model);
                }

                var otp = Random.Shared.Next(100000, 999999).ToString();
                user.Otp = otp;
                user.OtpExpiry = DateTime.UtcNow.AddMinutes(10);
                await _context.SaveChangesAsync();

                try
                {
                    await _emailService.SendOtpEmail(user.Email, otp);
                    TempData["SuccessMessage"] = "OTP has been sent to your email address.";
                }
                catch (EmailServiceException)
                {
                    // Log the error but don't expose it to the user
                    _logger.LogError("Failed to send OTP email to {Email}", user.Email);
                    ModelState.AddModelError("", "Unable to send OTP at the moment. Please try again later.");
                    return View(model);
                }

                TempData["ResetEmail"] = user.Email;
                return RedirectToAction("VerifyResetOtp");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ForgotPassword action");
                ModelState.AddModelError("", "An error occurred. Please try again later.");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult VerifyResetOtp()
        {
            var email = TempData["ResetEmail"]?.ToString();
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("ForgotPassword");
            }
            ViewBag.Email = email;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerifyResetOtp(string email, string otp)
        {
            var user = await _context.UserAccounts.FirstOrDefaultAsync(u => u.Email == email);
            
            if (user == null || string.IsNullOrEmpty(otp))
            {
                TempData["ErrorMessage"] = "Invalid request.";
                return RedirectToAction("ForgotPassword");
            }

            if (user.Otp != otp || DateTime.UtcNow > user.OtpExpiry)
            {
                ModelState.AddModelError("", "Invalid OTP or OTP has expired");
                ViewBag.Email = email;
                return View();
            }

            // Clear OTP after successful verification
            user.Otp = null;
            user.OtpExpiry = null;
            await _context.SaveChangesAsync();

            ViewBag.Email = email;
            TempData["ResetEmail"] = email; // Store email for the reset password page
            return RedirectToAction("ResetPassword", new { email = email });
        }

        [HttpGet]
        public IActionResult ResetPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("ForgotPassword");
            }
            ViewBag.Email = email;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Email = model.Email;
                return View(model);
            }

            var user = await _context.UserAccounts.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid request.");
                return View(model);
            }

            // Update password
            user.Password = PasswordHasher.HashPassword(model.NewPassword);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Your password has been reset successfully. Please login with your new password.";
            return RedirectToAction("Login");
        }

        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = await _context.UserAccounts.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null || !PasswordHasher.VerifyPassword(user.Password, model.CurrentPassword))
            {
                ModelState.AddModelError("CurrentPassword", "Current password is incorrect");
                return View(model);
            }

            user.Password = PasswordHasher.HashPassword(model.NewPassword);
            await _context.SaveChangesAsync();

            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult VerifyEmail(string email)
        {
            ViewBag.Email = email;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerifyEmail(string email, string otp)
        {
            var user = await _context.UserAccounts.FirstOrDefaultAsync(u => u.Email == email);
            
            if (user != null && user.Otp == otp && DateTime.UtcNow <= user.OtpExpiry)
            {
                user.IsVerified = true;
                await _context.SaveChangesAsync();
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", "Invalid OTP or OTP has expired");
            ViewBag.Email = email;
            return View();
        }
        //[HttpPost]
        //public async Task<IActionResult> AcceptUser(int Id)
        //{
        //    var user = await _context.UserAccounts.FindAsync(Id);
        //    if (user != null)
        //    {
        //        user.IsVerified = true;
        //        await _context.SaveChangesAsync();
        //        TempData["Success"] = "User has been verified.";
        //    }
        //    return RedirectToAction("ViewUsers");
        //}

        //[HttpPost]
        //public async Task<IActionResult> RejectUser(int id)
        //{
        //    var user = await _context.UserAccounts.FindAsync(id);
        //    if (user != null)
        //    {
        //        _context.UserAccounts.Remove(user);
        //        await _context.SaveChangesAsync();
        //        TempData["Success"] = "User has been rejected and deleted.";
        //    }
        //    return RedirectToAction("ViewUsers");
        //}

        //[Authorize]
        //public async Task<IActionResult> ProfileManage()
        //{
        //    var email = User.Identity?.Name ?? User.FindFirstValue(ClaimTypes.Email);

        //    if (string.IsNullOrEmpty(email))
        //    {
        //        ViewBag.Message = "Email not found in identity.";
        //        return View("Manage", null);
        //    }

        //    // Find user and include bookings with related farmhouse info
        //    var user = _context.UserAccounts
        //        .Include(u => u.Bookings)
        //        .ThenInclude(b => b.FarmHouse)
        //        .FirstOrDefault(u => u.Email == email);

        //    if (user == null)
        //    {
        //        ViewBag.Message = "User not found.";
        //        return View("Manage", null);
        //    }

        //    return View("Manage", user);
        //}
        public async Task<IActionResult> ProfileManage()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var bookings = await _context.tblBooking
                .Include(b => b.FarmHouse)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            return View(bookings);
        }
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
