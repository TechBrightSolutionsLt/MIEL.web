using Microsoft.AspNetCore.Mvc;

namespace MIEL.web.Views.Admin
{
    public class AdminController : Controller
    {
        public IActionResult index()
        {
            return RedirectToAction("AdminDashboard");
        }

        public IActionResult AdminDashboard()
        {
            return View();
        }
    }
}
