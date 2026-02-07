using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MIEL.web.Models;

using MIEL.web.Data;
using MIEL.web.Models.ViewModel;

namespace MIEL.web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDBContext _context;

        public HomeController(ILogger<HomeController> logger, AppDBContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.BannerImages = _context.ImageItems
                                          .OrderByDescending(x => x.Id)
                                          .ToList();

            var categories = _context.Categories
           .Select(c => new indexcategoryVM
           {
               CategoryId = c.CategoryId,
               CategoryName = c.CategoryName,

               ImagePath = _context.ProductImages
                   .Where(pi =>
                       pi.ProductId ==
                       _context.ProductMasters
                           .Where(p => p.CategoryId == c.CategoryId)
                           .OrderByDescending(p => p.CreatedDate)
                           .Select(p => p.ProductId)
                           .FirstOrDefault()
                   )
                   .OrderByDescending(pi => pi.ImgId)   
                   .Select(pi => pi.ImgPath)
                   .FirstOrDefault()
           })
           .ToList();

            ViewBag.indexcategoryVM = categories;

            return View();
        }

        public IActionResult CategoryProducts(int categoryId)
        {
            var products = _context.ProductMasters
                .Where(p => p.CategoryId == categoryId)
                .Select(p => new ProductListVM
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Brand = p.Brand,

                    ImagePath = _context.ProductImages
                        .Where(i => i.ProductId == p.ProductId)
                        .OrderByDescending(i => i.ImgId)
                        .Select(i => i.ImgPath)
                        .FirstOrDefault()
                })
                .ToList();

            return View(products);
        }




        public IActionResult ProductDetails(int id)
        {
            var product = _context.ProductMasters
                .Where(p => p.ProductId == id)
                .Select(p => new ProductListVM
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Brand = p.Brand,
                    ProductDescription = p.ProductDescription,

                    ImagePath = _context.ProductImages
                        .Where(i => i.ProductId == p.ProductId)
                        .OrderByDescending(i => i.ImgId)
                        .Select(i => i.ImgPath)
                        .FirstOrDefault(),

                    Images = _context.ProductImages
                        .Where(i => i.ProductId == p.ProductId)
                        .OrderByDescending(i => i.ImgId)
                        .Select(i => i.ImgPath)
                        .ToList()
                })
                .FirstOrDefault();

            if (product == null)
                return NotFound();

            return View(product);
        }




        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        public IActionResult ShopProducts()
        {
            return View("~/Views/Home/CategoryList.cshtml");
        }
        public IActionResult Admin()
        {
            return View("~/Views/Admin/AdminDashboard.cshtml");
        }
    }
}
