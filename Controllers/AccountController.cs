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

            // GET GUEST ID FROM COOKIE
            string guestId = Request.Cookies["GuestId"];

            // ✅ TRANSFER GUEST CART TO CUSTOMER CART
            if (!string.IsNullOrEmpty(guestId))
            {
                var guestCartItems = _context.Cart
                    .Where(c => c.GuestId == guestId)
                    .ToList();

                foreach (var item in guestCartItems)
                {
                    // check if same variant already exists for customer
                    var existing = _context.Cart.FirstOrDefault(x =>
                        x.CustomerId == user.CustomerId &&
                        x.VariantId == item.VariantId);

                    if (existing != null)
                    {
                        // merge quantity
                        existing.Quantity += item.Quantity;

                        // remove guest row
                        _context.Cart.Remove(item);
                    }
                    else
                    {
                        // assign customer id
                        item.CustomerId = user.CustomerId;
                        item.GuestId = null;
                    }
                }

                _context.SaveChanges();
            }

            // SET SESSION
            HttpContext.Session.SetString("CustomerId", user.CustomerId.ToString());
            HttpContext.Session.SetString("UserName", user.FirstName);
            HttpContext.Session.SetString("RoleId", user.RoleId.ToString());

            return user.RoleId switch
            {
                1 => RedirectToAction("Admin", "Home"),
                2 => RedirectToAction("Index", "Home"),
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
