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
                return View("~/Views/Home/Login.cshtml", model);

            var user = _context.users_TB.FirstOrDefault(x =>
                (x.Email == model.Username || x.MobileNumber == model.Username) &&
                x.Password == model.Password
            );

            if (user == null)
            {
                ViewBag.Error = "Invalid email/mobile or password";
                return View("~/Views/Home/Login.cshtml", model);
            }

            // Store user info in session
            HttpContext.Session.SetString("CustomerId", user.CustomerId.ToString());
            HttpContext.Session.SetString("UserName", user.FirstName);
            HttpContext.Session.SetString("RoleId", user.RoleId.ToString());

            // Redirect based on role
            return user.RoleId switch
            {
                1 => RedirectToAction("Admin", "Home"),
                2 => RedirectToAction("Profile", "Account"),
                _ => RedirectToAction("Index", "Home")
            };
        }

        // GET: Profile
        [HttpGet]
        public IActionResult Profile()
        {
            var customerId = HttpContext.Session.GetString("CustomerId");
            if (customerId == null)
                return RedirectToAction("Login");

            int id = int.Parse(customerId);

            var user = _context.users_TB.FirstOrDefault(x => x.CustomerId == id);
            if (user == null)
                return RedirectToAction("Login");

            // Map user data to view model (exclude password)
            var model = new UserProfileVM
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                MobileNumber = user.MobileNumber,
                Gender = user.Gender,
                Address = user.Address,
                City = user.City,
                Postcode = user.Postcode
            };

            return View(model);
        }

        // POST: Profile (update)
        [HttpPost]
        public IActionResult Profile(UserProfileVM model)
        {
            var customerId = HttpContext.Session.GetString("CustomerId");
            if (customerId == null)
                return RedirectToAction("Login");

            if (!ModelState.IsValid)
                return View(model);

            int id = int.Parse(customerId);

            var user = _context.users_TB.FirstOrDefault(x => x.CustomerId == id);
            if (user == null)
                return RedirectToAction("Login");

            // Update user fields
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.MobileNumber = model.MobileNumber;
            user.Gender = model.Gender;
            user.Address = model.Address;
            user.City = model.City;
            user.Postcode = model.Postcode;

            _context.SaveChanges();

            ViewBag.Success = "Profile updated successfully!";
            return View(model);
        }

        // Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
