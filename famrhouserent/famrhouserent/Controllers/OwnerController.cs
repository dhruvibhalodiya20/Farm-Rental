using famrhouserent.Data;
using famrhouserent.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; 
using Microsoft.AspNetCore.Http;    


namespace famrhouserent.Controllers
{
    [AllowAnonymous]
    public class OwnerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OwnerController> _logger;

        public OwnerController(ApplicationDbContext context, ILogger<OwnerController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ------------------ REGISTER ------------------
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Owner owner)
        {
            if (_context.tblOwner.Any(o => o.Email == owner.Email))
            {
                ModelState.AddModelError("Email", "Email already exists.");
                return View(owner);
            }

            _context.tblOwner.Add(owner);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Registration successful. Please login.";
            return RedirectToAction("Login");
        }

        // ------------------ LOGIN ------------------
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var owner = _context.tblOwner.FirstOrDefault(o => o.Email == email && o.Password == password);

            if (owner != null)
            {
                _logger.LogInformation($"Owner {owner.FullName} logged in successfully.");

                HttpContext.Session.SetInt32("OwnerId", owner.OwnerId);  
                HttpContext.Session.SetString("OwnerName", owner.FullName);

                return RedirectToAction("Dashboard");
            }

            _logger.LogWarning("Invalid login attempt.");
            ViewBag.Error = "Invalid login credentials.";
            return View();
        }



        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // ------------------ DASHBOARD ------------------
        public IActionResult Dashboard()
        {
            // Optional: Check if logged in
            var ownerId = HttpContext.Session.GetInt32("OwnerId");
            if (ownerId == null)
                return RedirectToAction("Login");

            return View();
        }


        // ------------------ ADD FARM ------------------
        [HttpGet]
        public IActionResult AddFarm()
        {
            if (HttpContext.Session.GetInt32("OwnerId") == null)
                return RedirectToAction("Login");

            return View();
        }

        //[HttpPost]
        //public async Task<IActionResult> AddFarm(FarmHouse model)
        //{
        //    int? ownerId = HttpContext.Session.GetInt32("OwnerId");
        //    if (ownerId == null) return RedirectToAction("Login");

        //    model.OwnerId = ownerId.Value;
        //    _context.FarmHouses.Add(model);
        //    await _context.SaveChangesAsync();

        //    TempData["Success"] = "Farm added successfully!";
        //    return RedirectToAction("Dashboard");
        //}

        [HttpPost]
        public async Task<IActionResult> AddFarm(FarmHouse model)
        {
            int? ownerId = HttpContext.Session.GetInt32("OwnerId");
            if (ownerId == null) return RedirectToAction("Login");

            model.OwnerId = ownerId.Value;

            _context.FarmHouses.Add(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Farm added successfully!";

            // Redirect to FarmHouseController -> Details
            return RedirectToAction("Details", "FarmHouse", new { id = model.Id });
        }


        // ------------------ DELETE FARM ------------------
        public async Task<IActionResult> DeleteFarm(int id)
        {
            var farm = await _context.FarmHouses.FindAsync(id);
            if (farm != null)
            {
                _context.FarmHouses.Remove(farm);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Dashboard");
        }

        public IActionResult YourFarms()
        {
            var ownerId = HttpContext.Session.GetInt32("OwnerId");
            if (ownerId == null)
                return RedirectToAction("Login");

            var farms = _context.FarmHouses.Where(f => f.OwnerId == ownerId.Value).ToList();

            return View(farms); 
        }

    }

}
