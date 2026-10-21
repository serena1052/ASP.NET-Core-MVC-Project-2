using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using _1131605.Data;
using _1131605.Models;

namespace _1131605.Controllers
{
    public class ItemController : Controller
    {
        private readonly AppDbContext _context;

        public ItemController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string searchString = "")
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("User")))
            {
                return RedirectToAction("Login", "Account");
            }

            var itemsQuery = from i in _context.Items select i;

            if (!string.IsNullOrEmpty(searchString))
            {
                itemsQuery = itemsQuery.Where(i => i.Name.Contains(searchString) || i.Category.Contains(searchString));
            }

            var items = itemsQuery.OrderBy(i => i.ItemId).ToList();

            ViewBag.CurrentSearch = searchString;

            return View(items);
        }

        public IActionResult Create()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("User")))
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Create(Item item)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("User")))
            {
                return RedirectToAction("Login", "Account");
            }

            item.Rarity = "普通";
            item.IsBanned = false;

            ModelState.Remove("Rarity");
            ModelState.Remove("IsBanned");
            ModelState.Remove("ItemId"); 

            if (ModelState.IsValid)
            {
                try
                {
                    if (!_context.Items.Any())
                    {
                        item.ItemId = 1;
                    }
                    else
                    {
                        int maxId = _context.Items.Max(i => i.ItemId);
                        item.ItemId = maxId + 1;
                    }

                    _context.Items.Add(item);
                    _context.SaveChanges(); 

             
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "資料庫寫入失敗：" + ex.Message);
                }
            }
            return View(item);
        }

        public IActionResult Edit(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("User")))
            {
                return RedirectToAction("Login", "Account");
            }

            var item = _context.Items.Find(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        [HttpPost]
        public IActionResult Edit(int id, Item item)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("User")))
            {
                return RedirectToAction("Login", "Account");
            }

            if (id != item.ItemId)
            {
                return BadRequest();
            }

            ModelState.Remove("Rarity");
            ModelState.Remove("IsBanned");

            if (ModelState.IsValid)
            {
                try
                {
                    var dbItem = _context.Items.Find(id);
                    if (dbItem == null) return NotFound();

                    dbItem.Name = item.Name;
                    dbItem.Category = item.Category;
                    dbItem.Price = item.Price;
                    dbItem.Rating = item.Rating;

                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "資料庫更新失敗：" + ex.Message);
                }
            }
            return View(item);
        }

        public IActionResult Delete(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("User")))
            {
                return RedirectToAction("Login", "Account");
            }

            var item = _context.Items.Find(id);
            if (item != null)
            {
                _context.Items.Remove(item);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}