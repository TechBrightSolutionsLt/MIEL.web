using Microsoft.AspNetCore.Mvc;

namespace MIEL.web.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
