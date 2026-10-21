using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using _1131605.Data;

namespace _1131605.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (username == "abcd" && password == "abcd1122")
            {

                HttpContext.Session.SetString("User", username);
                HttpContext.Session.SetInt32("UserId", 1);

                return RedirectToAction("Index", "Decision");
            }

            ModelState.AddModelError("", "帳號或密碼錯誤");
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}