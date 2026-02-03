using Microsoft.AspNetCore.Mvc;
using MIEL.web.Data;
using MIEL.web.Models.ViewModel;

namespace MIEL.web.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDBContext _context;

        public AccountController(AppDBContext context)
        {
            _context = context;
        }

        // GET: Login
        [HttpGet]
        public IActionResult Login()
        {
            return View("~/Views/Home/Login.cshtml");
        }

        // POST: Login
        [HttpPost]
        public IActionResult Login(UserLoginVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _context.users_TB.FirstOrDefault(x =>
                (x.Email == model.Username || x.MobileNumber == model.Username)
                && x.Password == model.Password
            );

            if (user == null)
            {
                ViewBag.Error = "Invalid email/mobile or password";
                return View(model);
            }

            //  Login success
            HttpContext.Session.SetString("CustomerId", user.CustomerId.ToString());
            HttpContext.Session.SetString("UserName", user.FirstName);
            HttpContext.Session.SetString("RoleId", user.RoleId.ToString());

            int roleid = user.RoleId;

            if (roleid == 1)
            {
                return View("~/Views/Admin/AdminDashboard.cshtml");
            }
            else if (roleid == 2)
            {
                return RedirectToAction("Index", "Home");
            }

            // fallback (important)
            return RedirectToAction("Index", "Home");
        }

        // Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
