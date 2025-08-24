using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using famrhouserent.Models;
using famrhouserent.Models.ViewModels;
using famrhouserent.Data;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace famrhouserent.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        ViewBag.Locations = new List<SelectListItem>
        {
            new SelectListItem { Text = "Gaypagla", Value = "Gaypagla" },
            new SelectListItem { Text = "Sevni", Value = "Sevni" },
            
        };
        var farmhouses = _context.FarmHouses.ToList();
        return View(farmhouses);
    }

    public IActionResult Properties()
    {
        var farmhouses = _context.FarmHouses.ToList(); 
        return View(farmhouses); 
    }


    public IActionResult About()
    {
        return View();
    }

    //[HttpGet]
    //public IActionResult Contact()
    //{
    //    return View(new ContactViewModel());
    //}

    public async Task<IActionResult> Contact()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        int? userId = null;

        if (int.TryParse(userIdString, out var parsedId))
        {
            userId = parsedId;
        }

        var previousQueries =  _context.Contacts
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .ToList();

        ViewBag.PreviousQueries = previousQueries;

        return View(new ContactViewModel());
    }




    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> Contact(ContactViewModel model)
    //{
    //    if (!ModelState.IsValid)
    //    {
    //        return View(model);
    //    }

    //    var contact = new Contact
    //    {
    //        Name = model.Name,
    //        Email = model.Email,
    //        Phone = model.Phone,
    //        Subject = model.Subject,
    //        Message = model.Message
    //    };

    //    _context.Contacts.Add(contact);
    //    await _context.SaveChangesAsync();

    //    TempData["SuccessMessage"] = "Thank you for your message. We'll get back to you soon!";
    //    return RedirectToAction(nameof(Contact));
    //}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Contact(ContactViewModel model)
    {
        if (ModelState.IsValid)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int? userId = null;

            if (int.TryParse(userIdString, out var parsedId))
            {
                userId = parsedId;
            }

            var contact = new Contact
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                Subject = model.Subject,
                Message = model.Message,
                CreatedAt = DateTime.UtcNow,
                IsRead = false,
                UserId = userId 
            };

            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Your message has been sent!";
            return RedirectToAction(nameof(Contact));
        }

        return View(model);
    }




    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
