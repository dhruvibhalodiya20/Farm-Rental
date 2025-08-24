using System.Security.Claims;
using famrhouserent.Data;
using famrhouserent.Models;
using Microsoft.AspNetCore.Mvc;

namespace famrhouserent.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FeedbackController(ApplicationDbContext context)
        {
            _context = context;
        }

        //// GET: Feedback/Create
        //public IActionResult Create()
        //{
        //    var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        //    if (!int.TryParse(userId, out var parsedUserId))
        //    {
        //        TempData["Error"] = "Unable to identify user.";
        //        return RedirectToAction("Login", "Auth");
        //    }

        //    var booking = _context.tblBooking
        //        .Where(b => b.UserId == parsedUserId)
        //        .OrderByDescending(b => b.Id)
        //        .FirstOrDefault();

        //    if (booking == null)
        //    {
        //        TempData["Error"] = "You must have a booking to submit feedback.";
        //        return RedirectToAction("Index", "Home");
        //    }

        //    var feedback = new Feedback { BookingId = booking.Id };
        //    return View(feedback);
        //}

        //// POST: Feedback/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Create(Feedback feedback)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.tblFeedback.Add(feedback);  // Save feedback to the database
        //        _context.SaveChanges();  // Commit the changes to the database
        //        TempData["Success"] = "Thank you for your feedback!";  // Set success message
        //        return RedirectToAction("Index", "Home");  // Redirect to home page
        //    }
        //    return View(feedback);  // If validation fails, return the view with the feedback model
        //}
        //-------------------------------------
        //[HttpGet]
        //public IActionResult Create()
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        //    if (!int.TryParse(userId, out var parsedUserId))
        //    {
        //        TempData["Error"] = "Unable to identify user.";
        //        return RedirectToAction("Login", "Auth");
        //    }

        //    var feedback = new Feedback
        //    {
        //        UserId = parsedUserId
        //    };

        //    return View(feedback);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Create(Feedback feedback)
        //{

        //        TempData["Success"] = "Thank you! Your feedback has been submitted.";
        //        return RedirectToAction("Index", "Home");


        //    //return View(feedback);
        //}

        public IActionResult Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userId, out var parsedUserId))
            {
                TempData["Error"] = "Unable to identify user.";
                return RedirectToAction("Login", "Auth");
            }

            var feedback = new Feedback
            {
                UserId = parsedUserId
            };

            return View(feedback);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Feedback feedback)
        {
            Console.WriteLine($"UserId: {feedback.UserId}, Rating: {feedback.Rating}");

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Validation failed: " + string.Join(" | ",
                 ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));

                try
                {
                    _context.tblFeedback.Add(feedback);
                    _context.SaveChanges();

                    TempData["Success"] = "Thank you! Your feedback has been submitted.";
                    return RedirectToAction("Create"); 
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Error saving feedback: " + ex.Message;
                }
            }

            foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine("Model Error: " + modelError.ErrorMessage);
            }

            return View(feedback);
        }


    }
}
