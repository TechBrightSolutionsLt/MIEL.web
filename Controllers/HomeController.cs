using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MIEL.web.Models;

using MIEL.web.Data;
using MIEL.web.Models.ViewModel;
using Newtonsoft.Json;

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
                      pi.Flag == 1 &&                     // ? only main image
                      pi.ProductId ==
                          _context.ProductMasters
                              .Where(p => p.CategoryId == c.CategoryId)
                              .OrderByDescending(p => p.CreatedDate)
                              .Select(p => p.ProductId)
                              .FirstOrDefault()
                  )
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
    .Where(i => i.ProductId == p.ProductId && i.Flag == 1)
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

                    // Main image
                    ImagePath = _context.ProductImages
                        .Where(i => i.ProductId == p.ProductId && i.Flag == 1)
                        .Select(i => i.ImgPath)
                        .FirstOrDefault(),

                    // Other images
                    Images = _context.ProductImages
                        .Where(i => i.ProductId == p.ProductId && i.Flag == 0)
                        .OrderBy(i => i.ImgId)
                        .Select(i => i.ImgPath)
                        .ToList(),

                    // Correct specification join
                    Specificationsnew = (
                        from ps in _context.productspecifications
                        join s in _context.Specifications
                            on ps.Id equals s.Id
                        where ps.ProductId == p.ProductId
                        select new SpecificationVM
                        {
                            SpecName = s.SpecName,
                            SpecValue = ps.specificationvalue
                        }
                    ).ToList(),

                    // Variants
                    Variants = _context.ProColorSizeVariants
                        .Where(v => v.ProductId == p.ProductId)
                        .Select(v => new ColorSizeVM
                        {
                            Color = v.colour,
                            Size = v.size
                        })
                        .ToList()
                })
                .FirstOrDefault();

            if (product == null)
                return NotFound();

            return View(product);
        }


        [HttpPost]
        public IActionResult AddToCart([FromBody] CartItem item)
        {
            var cartJson = HttpContext.Session.GetString("Cart");
            List<CartItem> cart;

            if (string.IsNullOrEmpty(cartJson))
            {
                cart = new List<CartItem>();
            }
            else
            {
                cart = JsonConvert.DeserializeObject<List<CartItem>>(cartJson);
            }


            var existing = cart.FirstOrDefault(x =>
     x.ProductId == item.ProductId && x.Size == item.Size
 );

            if (existing != null)
            {
                existing.Quantity += item.Quantity;
            }
            else
            {
                cart.Add(item);
            }

            HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));

            return Json(new { success = true });
        }

        [HttpGet]
        public IActionResult GetCartCount()
        {
            int count = 0;

            var cart = HttpContext.Session.GetString("Cart");
            if (!string.IsNullOrEmpty(cart))
            {
                var cartItems = JsonConvert.DeserializeObject<List<CartItem>>(cart);
                count = cartItems.Sum(x => x.Quantity);
            }

            return Json(new { count });
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
