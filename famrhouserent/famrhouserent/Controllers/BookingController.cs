using famrhouserent.Data;
using famrhouserent.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace famrhouserent.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ManageBooking()
        {
            var bookings = _context.tblBooking
                .Include(b => b.UserAccount)
                .Include(b => b.FarmHouse)
                .ToList();

            return View(bookings);
        }
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var booking = await _context.tblBooking.FindAsync(id);
            if (booking != null && (status == "Confirmed" || status == "Cancelled"))
            {
                booking.BookingStatus = status;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("ManageBooking");
        }

        public async Task<IActionResult> ConfirmedOrCancelled()
        {
            var confirmed = await _context.tblBooking
                .Include(b => b.UserAccount)
                .Include(b => b.FarmHouse)
                .Where(b => b.BookingStatus == "Confirmed")
                .ToListAsync();

            var cancelled = await _context.tblBooking
                .Include(b => b.UserAccount)
                .Include(b => b.FarmHouse)
                .Where(b => b.BookingStatus == "Cancelled")
                .ToListAsync();

            var model = new Tuple<List<Booking>, List<Booking>>(confirmed, cancelled);
            return View(model);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var booking = await _context.tblBooking
                .Include(b => b.UserAccount)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
                return NotFound();

            if (booking.UserAccount.Email != User.Identity.Name)
                return Forbid();

            booking.BookingStatus = "Cancelled";
            _context.Update(booking);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Booking cancelled successfully.";
            return RedirectToAction("MyBookings");
        }
        public IActionResult BookingConfirmation(int id, bool success = false)
        {
            var booking = _context.tblBooking.FirstOrDefault(b => b.Id == id);
            ViewBag.Success = success;
            return View(booking);
        }




    }
}

