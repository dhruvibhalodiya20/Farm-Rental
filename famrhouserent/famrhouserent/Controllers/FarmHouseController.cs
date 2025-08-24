using famrhouserent.Data;
using famrhouserent.Models;
using famrhouserent.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace famrhouserent.Controllers
{
    public class FarmHouseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FarmHouseController(ApplicationDbContext context)
        {
            _context = context;
        }

         [HttpGet]
        [Route("FarmHouse/Details/{id}")]
        public IActionResult Details(int id)
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
                Description = farmhouse.Description,
                Bedrooms = farmhouse.Bedrooms,
                MaxGuests = farmhouse.MaxGuests,
                PricePerNight = farmhouse.PricePerNight,
                Amenities = farmhouse.Amenities,
                ImageUrl = farmhouse.FarmHouseImages.FirstOrDefault()?.ImageUrl ?? "~/images/noimage.png",
                ImageUrls = farmhouse.FarmHouseImages.Select(img => img.ImageUrl).Skip(1).ToList()
            };

            return View(viewModel);
        }


        [HttpPost]
        public IActionResult Book(int id, DateTime checkIn, DateTime checkOut, int guests)
        {
            int userId = 1; // hardcoded for now

            if (checkOut <= checkIn)
            {
                TempData["Error"] = "Check-out date must be after check-in date.";
                return RedirectToAction("Details", new { id });
            }

            var isBooked = _context.tblBooking.Any(b =>
                b.FarmHouseId == id &&
                b.BookingStatus != "Cancelled" &&
                ((checkIn >= b.FromDate && checkIn < b.ToDate) ||
                 (checkOut > b.FromDate && checkOut <= b.ToDate) ||
                 (checkIn <= b.FromDate && checkOut >= b.ToDate))
            );

            if (isBooked)
            {
                TempData["Error"] = "This farmhouse is already booked for the selected dates.";
                return RedirectToAction("Details", new { id });
            }

            try
            {
                var farm = _context.FarmHouses.FirstOrDefault(f => f.Id == id);

                var totalDays = (checkOut - checkIn).Days;
                var totalPrice = totalDays * farm.PricePerNight;

                var booking = new Booking
                {
                    FarmHouseId = id,
                    UserId = userId,
                    FromDate = checkIn,
                    ToDate = checkOut,
                    Guests = guests,
                    TotalPrice = totalPrice,
                    BookingDate = DateTime.Now,
                    BookingStatus = "Pending"
                };

                _context.tblBooking.Add(booking);
                _context.SaveChanges();

                TempData["Success"] = "Booking successful!";
                return RedirectToAction("Details", new { id });
            }
            catch (Exception ex)
            {
                // Log the error (ex.ToString()) for dev purposes
                TempData["Error"] = "Something went wrong while booking. Please try again.";
                return RedirectToAction("Details", new { id });
            }
        }



        public IActionResult Filter(string location, string price, string amenities)
        {
            var farmhouses = _context.FarmHouses.AsQueryable();

            if (!string.IsNullOrEmpty(location))
                farmhouses = farmhouses.Where(f => f.Location == location);

            if (!string.IsNullOrEmpty(price))
            {
                if (price == "0-5000")
                    farmhouses = farmhouses.Where(f => f.PricePerNight < 5000);
                else if (price == "5000-10000")
                    farmhouses = farmhouses.Where(f => f.PricePerNight >= 5000 && f.PricePerNight <= 10000);
                else if (price == "10000+")
                    farmhouses = farmhouses.Where(f => f.PricePerNight > 10000);
            }

            if (!string.IsNullOrEmpty(amenities))
                farmhouses = farmhouses.Where(f => f.Amenities.Contains(amenities));

            var list = farmhouses.ToList();

            if (!list.Any())
                return Content("<p class='no-farms'>No farmhouses found for the selected filters.</p>");

            return PartialView("_FarmHousePartial", list);
        }
        public IActionResult RevenueReport(int farmhouseId)
        {
            var bookings = _context.tblBooking
                .Where(b => b.FarmHouseId == farmhouseId && b.BookingStatus == "Confirmed")
                .ToList();

            var totalRevenue = bookings.Sum(b => b.TotalPrice ?? 0);
            var totalBookings = bookings.Count;

            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.TotalBookings = totalBookings;
            ViewBag.FarmHouseName = _context.FarmHouses.FirstOrDefault(f => f.Id == farmhouseId)?.Name;

            return View(bookings); 
        }


    }

}
