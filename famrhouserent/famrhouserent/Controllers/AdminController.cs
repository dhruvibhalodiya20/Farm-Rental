using famrhouserent.Data;
using famrhouserent.Models;
using famrhouserent.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;

namespace famrhouserent.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View(); // Admin dashboard
        }

        [HttpGet]
        public IActionResult AddUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Check for existing email
            if (_context.UserAccounts.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email is already registered.");
                return View(model);
            }

            // Hash password
            string salt = Convert.ToBase64String(RandomNumberGenerator.GetBytes(128 / 8));
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: model.Password,
                salt: Convert.FromBase64String(salt),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                Password = model.Password,
                IsAdmin = false
            };


            _context.UserAccounts.Add(user);
            await _context.SaveChangesAsync();

            TempData["Success"] = "User account created successfully.";
            return RedirectToAction("Index");
        }

        public IActionResult ViewUsers()
        {
            var users = _context.UserAccounts
                .Where(u => !u.IsAdmin)
                .OrderByDescending(u => u.Id)
                .Select(u => new AuthViewModel
                {
                    Name = u.Name,
                    Email = u.Email,
                    IsVerified = u.IsVerified
                })
                .ToList();

            return View(users);
        }

        public IActionResult ManageFarms()
        {
            var farms = _context.FarmHouses.ToList();
            return View(farms);
        }



        //[HttpPost]
        //public async Task<IActionResult> Add(FarmHouse model)
        //{
        //    if (!ModelState.IsValid)
        //        return View(model);

        //    _context.FarmHouses.Add(model);
        //    await _context.SaveChangesAsync();
        //    TempData["Success"] = "Farm added successfully!";
        //    return RedirectToAction("ManageFarms", "Admin");
        //}
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(FarmHouseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); // Return to view if there are validation errors
            }

            try
            {
                // Create the FarmHouse object from the view model
                var farmhouse = new FarmHouse
                {
                    Name = model.Name,
                    Location = model.Location,
                    Description = model.Description,
                    Bedrooms = model.Bedrooms,
                    MaxGuests = model.MaxGuests,
                    PricePerNight = model.PricePerNight,
                    Amenities = model.Amenities,
                    FarmHouseImages = new List<FarmHouseImage>()
                };

                // Define the path to save the uploaded images
                var imagesFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");

                // Handle image uploads (if any)
                if (model.Images != null && model.Images.Count > 0)
                {
                    foreach (var image in model.Images)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName); // Generate a unique name for the image
                        var filePath = Path.Combine(imagesFolder, fileName); // Define the full file path

                        // Save the image to the server
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        // Add the image URL to the FarmHouseImages collection
                        farmhouse.FarmHouseImages.Add(new FarmHouseImage
                        {
                            ImageUrl = "/images/" + fileName // URL to access the image
                        });
                    }
                }

                // Add the FarmHouse object to the database
                _context.FarmHouses.Add(farmhouse);
                await _context.SaveChangesAsync(); // Save changes to the database

                // Redirect to another action (for example, the Index page)
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log the error or handle it
                Console.WriteLine(ex.Message);
                return View(model); // Return the model to the view if something goes wrong
            }
        }






        [HttpGet]
        public IActionResult Edit(int id)
        {
            var farmhouse = _context.FarmHouses
                .Include(f => f.FarmHouseImages)
                .FirstOrDefault(f => f.Id == id);

            if (farmhouse == null) return NotFound();

            var viewModel = new FarmHouseViewModel
            {
                Id = farmhouse.Id,
                Name = farmhouse.Name,
                Location = farmhouse.Location,

                Bedrooms = farmhouse.Bedrooms,
                MaxGuests = farmhouse.MaxGuests,
                PricePerNight = farmhouse.PricePerNight,

                ExistingImages = farmhouse.FarmHouseImages.Select(img => img.ImageUrl).ToList()
            };

            return View(viewModel); // ✅ Now you're passing the correct type
        }


        //[HttpPost]
        //public async Task<IActionResult> Edit(FarmHouseViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //        return View(model);

        //    var farmhouse = await _context.FarmHouses.Include(f => f.FarmHouseImages).FirstOrDefaultAsync(f => f.Id == model.Id);
        //    if (farmhouse == null) return NotFound();

        //    // Update properties
        //    farmhouse.Name = model.Name;
        //    farmhouse.Location = model.Location;

        //    farmhouse.Bedrooms = model.Bedrooms;
        //    farmhouse.MaxGuests = model.MaxGuests;
        //    farmhouse.PricePerNight = model.PricePerNight;


        //    // Optional: handle new images if model.Images is not null

        //    await _context.SaveChangesAsync();
        //    TempData["Success"] = "Farm updated successfully!";
        //    return RedirectToAction("ManageFarms");
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(FarmHouseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var farmhouse = await _context.FarmHouses
                .Include(f => f.FarmHouseImages)
                .FirstOrDefaultAsync(f => f.Id == model.Id);

            if (farmhouse == null)
                return NotFound();

            // Update values
            farmhouse.Name = model.Name;
            farmhouse.Location = model.Location;
            farmhouse.Bedrooms = model.Bedrooms;
            farmhouse.MaxGuests = model.MaxGuests;
            farmhouse.PricePerNight = model.PricePerNight;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Farm updated successfully!";
            return RedirectToAction("ManageFarms");
        }


        //public IActionResult Edit(int id)
        //{
        //    var farm = _context.FarmHouses.Find(id);
        //    if (farm == null) return NotFound();

        //    return View(farm);
        //}
        //[HttpGet]
        //public IActionResult Edit(int id)
        //{
        //    var farmhouse = _context.FarmHouses
        //        .Include(f => f.FarmHouseImages)
        //        .FirstOrDefault(f => f.Id == id);

        //    if (farmhouse == null) return NotFound();

        //    var viewModel = new FarmHouseViewModel
        //    {
        //        Id = farmhouse.Id,
        //        Name = farmhouse.Name,
        //        Location = farmhouse.Location,
        //        Description = farmhouse.Description,
        //        Bedrooms = farmhouse.Bedrooms,
        //        MaxGuests = farmhouse.MaxGuests,
        //        PricePerNight = farmhouse.PricePerNight,
        //        Amenities = farmhouse.Amenities
        //    };

        //    return View(viewModel);
        //}


        // POST: /Farm/Edit/5
        //[HttpPost]
        //public async Task<IActionResult> Edit(FarmHouse model)
        //{
        //    if (!ModelState.IsValid)
        //        return View(model);

        //    _context.FarmHouses.Update(model);
        //    await _context.SaveChangesAsync();
        //    TempData["Success"] = "Farm updated successfully!";
        //    return RedirectToAction("Index", "Home");
        //}



        // GET: /Farm/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var farm = await _context.FarmHouses.Include(f => f.Bookings).FirstOrDefaultAsync(f => f.Id == id);
            if (farm == null) return NotFound();

            _context.tblBooking.RemoveRange(farm.Bookings);


            _context.FarmHouses.Remove(farm);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Farm and related bookings deleted successfully!";
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> VerifyUser(string email)
        {
            var user = await _context.UserAccounts.FirstOrDefaultAsync(u => u.Email == email);
            if (user != null)
            {
                user.IsVerified = true;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "User verified successfully.";
            }
            return RedirectToAction("ViewUsers");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> RejectUser(string email)
        {
            var user = await _context.UserAccounts.FirstOrDefaultAsync(u => u.Email == email);
            if (user != null)
            {
                _context.UserAccounts.Remove(user);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "User rejected and removed.";
            }
            return RedirectToAction("ViewUsers");
        }

        public async Task<IActionResult> CustomerQueries()
        {
            var queries = await _context.Contacts
                .OrderByDescending(c => c.CreatedAt)
                .Take(10)
                .ToListAsync();

            return View(queries);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact != null)
            {
                contact.IsRead = true;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("CustomerQueries");
        }
        [HttpGet]

        public IActionResult Reply(int id)

        {

            var contact = _context.Contacts.Find(id);

            if (contact == null)

                return NotFound();



            return View(contact);

        }
        [HttpPost]
        public IActionResult ReplyToQuery(int queryId, string adminReply)
        {
            var query = _context.Contacts.FirstOrDefault(c => c.Id == queryId);

            if (query != null)
            {
                query.AdminReply = adminReply;
                query.IsRead = true; 
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Your reply has been sent successfully!";
            }
            return RedirectToAction("CustomerQueries");
        }
        public async Task<IActionResult> ViewFeedback()
        {
            var feedbackList = await _context.tblFeedback
                                             .Include(f => f.UserAccounts) 
                                             .OrderByDescending(f => f.SubmittedAt) 
                                             .ToListAsync();

            return View(feedbackList);
        }


    }
}
