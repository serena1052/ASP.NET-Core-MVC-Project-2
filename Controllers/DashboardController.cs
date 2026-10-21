using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;
using _1131605.Data;

namespace _1131605.Controllers
{
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("User")))
            {
                return RedirectToAction("Login", "Account");
            }

            var items = _context.Items.ToList();
            ViewBag.DinnerCount = items.Count(i => i.Category == "Dinner");
            ViewBag.ShopCount = items.Count(i => i.Category == "Shop");
            ViewBag.TravelCount = items.Count(i => i.Category == "Travel");

            ViewBag.PriceLow = items.Count(i => i.Price <= 150);                     
            ViewBag.PriceMedium = items.Count(i => i.Price > 150 && i.Price <= 500);    
            ViewBag.PriceHigh = items.Count(i => i.Price > 500);                   

            ViewBag.DinnerAvgRating = ViewBag.DinnerCount > 0 ? items.Where(i => i.Category == "Dinner").Average(i => i.Rating) : 0;
            ViewBag.ShopAvgRating = ViewBag.ShopCount > 0 ? items.Where(i => i.Category == "Shop").Average(i => i.Rating) : 0;
            ViewBag.TravelAvgRating = ViewBag.TravelCount > 0 ? items.Where(i => i.Category == "Travel").Average(i => i.Rating) : 0;

            ViewBag.DbStatus = "正常運作中";

            return View();
        }
    }
}