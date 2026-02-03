using Microsoft.AspNetCore.Mvc;
using MIEL.web.Models.ViewModel;
using MIEL.web.Services;

namespace MIEL.web.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserService _userService;

        // ✅ DI-based constructor
        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        // GET: Users/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Users/Register
        [HttpPost]
        public IActionResult Register(UserRegistrationVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                _userService.RegisterCustomer(model);
                return RedirectToAction("Success");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}
