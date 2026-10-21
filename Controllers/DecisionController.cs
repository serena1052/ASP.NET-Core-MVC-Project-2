using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Collections.Generic;
using _1131605.Data;
using _1131605.Services;
using _1131605.Models;

namespace _1131605.Controllers
{
    public class DecisionController : Controller
    {
        private readonly AppDbContext _context;
        private readonly RecommendationService _recommendService;

        public DecisionController(AppDbContext context, RecommendationService recommendService)
        {
            _context = context;
            _recommendService = recommendService;
        }

        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("User")))
            {
                return RedirectToAction("Login", "Account");
            }


            ViewBag.TopDinner = _recommendService.GetTop("Dinner").Take(2).ToList();
            ViewBag.TopShop = _recommendService.GetTop("Shop").Take(2).ToList();
            ViewBag.TopTravel = _recommendService.GetTop("Travel").Take(2).ToList();

            return View();
        }

        [HttpPost]
        public IActionResult Draw(string category, string strategy, int? maxPrice, double? minRating, string rollType)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("User")))
            {
                return RedirectToAction("Login", "Account");
            }

            var query = _context.Items.Where(i => i.Category == category && i.Rating >= 2.0);

            if (strategy == "HighRatingHighPrice")
            {
                query = query.Where(i => i.Rating >= 4.0).OrderByDescending(i => i.Price);
            }
            else if (strategy == "HighRatingLowPrice")
            {
                query = query.Where(i => i.Rating >= 4.0).OrderBy(i => i.Price);
            }
            else if (strategy == "LowRatingLowPrice")
            {
                query = query.Where(i => i.Rating < 4.0).OrderBy(i => i.Price);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(i => i.Price <= maxPrice.Value);
            }
            if (minRating.HasValue)
            {
                query = query.Where(i => i.Rating >= minRating.Value);
            }
            var candidateItems = query.ToList();

            if (candidateItems == null || candidateItems.Count == 0)
            {
                string categoryName = category == "Dinner" ? "晚餐選擇" :
                                     category == "Shop" ? "購買物品" : "旅遊景點";

                ViewBag.Error = $"在【{categoryName}】中，符合您指定的「象限策略」與「進階預算/評分」之商品數量為 0。請放寬限制或調整策略！";

                ViewBag.TopDinner = _recommendService.GetTop("Dinner").Take(2).ToList();
                ViewBag.TopShop = _recommendService.GetTop("Shop").Take(2).ToList();
                ViewBag.TopTravel = _recommendService.GetTop("Travel").Take(2).ToList();

                return View("Index");
            }

            _1131605.Models.Item resultItem = null;
            Random rand = new Random();


            if (rollType == "Weight")
            {
                double totalWeight = candidateItems.Sum(item => item.Rating);
                double randomValue = rand.NextDouble() * totalWeight;
                double currentSum = 0;

                foreach (var item in candidateItems)
                {
                    currentSum += item.Rating;
                    if (randomValue <= currentSum)
                    {
                        resultItem = item;
                        break;
                    }
                }
            }

            if (resultItem == null)
            {
                int randomIndex = rand.Next(candidateItems.Count);
                resultItem = candidateItems[randomIndex];
            }

            return View("Result", resultItem);
        }
    }
}