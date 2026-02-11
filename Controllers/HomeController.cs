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
            ViewBag.MainCategories = _context.MainCategories
      .Select(m => new
      {
          m.MainCategoryId,
          m.MainCategoryName
      })
      .ToList();

            return View();
           
        }
        [HttpGet]
        public IActionResult GetSubCategories(int categoryId)
        {
            var subCategories = _context.Categories
                .Where(c => c.MainCategoryId == categoryId)
                .Select(c => new
                {
                    subCategoryId = c.CategoryId,
                    subCategoryName = c.CategoryName
                })
                .ToList();

            return Json(subCategories);
        }


        public IActionResult CategoryProducts(int categoryId)
        {
            var products = (from p in _context.ProductMasters
                            where p.CategoryId == categoryId
                            select new ProductListVM
                            {
                                ProductId = p.ProductId,
                                ProductName = p.ProductName,
                                Brand = p.Brand,

                                ImagePath = _context.ProductImages
                                    .Where(i => i.ProductId == p.ProductId && i.Flag == 1)
                                    .Select(i => i.ImgPath)
                                    .FirstOrDefault(),

                                NetAmount = (from v in _context.ProColorSizeVariants
                                         join pi in _context.PurchaseItems
                                         on v.varientid equals pi.varientid
                                         where v.ProductId == p.ProductId
                                         orderby pi.PurchaseItemId descending
                                         select pi.Rate)
                                         .FirstOrDefault()
                            }).ToList();

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
                        .Where(i => i.ProductId == p.ProductId && i.Flag == 1)
                        .Select(i => i.ImgPath)
                        .FirstOrDefault(),

                    Images = _context.ProductImages
                        .Where(i => i.ProductId == p.ProductId && i.Flag == 0)
                        .OrderBy(i => i.ImgId)
                        .Select(i => i.ImgPath)
                        .ToList(),

                    Specificationsnew = (
                        from ps in _context.productspecifications
                        join s in _context.Specifications
                            on ps.Id equals s.Id
                        where ps.ProductId == p.ProductId
                        select new SpecificationVM
                        {
                            SpecName = s.SpecName,
                            SpecValue = ps.specificationvalue
                        }).ToList(),

                    // ? Correct Variants with Latest Rate
                    Variants = (
                        from v in _context.ProColorSizeVariants
                        where v.ProductId == p.ProductId
                        select new ColorSizeVM
                        {
                            VariantId = v.varientid,
                            Color = v.colour,
                            Size = v.size,
                            Rate = _context.PurchaseItems
                                .Where(pi => pi.varientid == v.varientid)
                                .OrderByDescending(pi => pi.PurchaseItemId)
                                .Select(pi => pi.Rate)
                                .FirstOrDefault()
                        }
                    ).ToList(),

                    // Default price (first variant)
                    NetAmount = _context.ProColorSizeVariants
                        .Where(v => v.ProductId == p.ProductId)
                        .Select(v => _context.PurchaseItems
                            .Where(pi => pi.varientid == v.varientid)
                            .OrderByDescending(pi => pi.PurchaseItemId)
                            .Select(pi => pi.Rate)
                            .FirstOrDefault()
                        )
                        .FirstOrDefault()
                })
                .FirstOrDefault();

            if (product == null)
                return NotFound();

            return View(product);
        }




        [HttpPost]
        public IActionResult AddToCart([FromBody] CartItem model)
        {
            var cartJson = HttpContext.Session.GetString("Cart");

            var cart = string.IsNullOrEmpty(cartJson)
                ? new List<CartItem>()
                : JsonConvert.DeserializeObject<List<CartItem>>(cartJson);

            // ?? Get variant with latest price
            var variant = (
                from v in _context.ProColorSizeVariants
                join pi in _context.PurchaseItems
                    on v.varientid equals pi.varientid
                where v.ProductId == model.ProductId
                      && v.colour == model.Color
                      && v.size == model.Size
                orderby pi.PurchaseItemId descending
                select new
                {
                    v.varientid,
                    pi.Rate
                }
            ).FirstOrDefault();

            if (variant == null)
                return Json(new { success = false });

            // ?? Get image
            var image = _context.ProductImages
                .Where(i => i.ProductId == model.ProductId && i.Flag == 1)
                .Select(i => i.ImgPath)
                .FirstOrDefault();

            var existingItem = cart.FirstOrDefault(x =>
                x.VariantId == variant.varientid);

            if (existingItem != null)
            {
                existingItem.Quantity += model.Quantity;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = model.ProductId,
                    VariantId = variant.varientid,
                    ProductName = _context.ProductMasters
                        .Where(p => p.ProductId == model.ProductId)
                        .Select(p => p.ProductName)
                        .FirstOrDefault(),

                    Color = model.Color,
                    Size = model.Size,
                    Price = variant.Rate,  // ? CORRECT PRICE
                    Image = image,         // ? CORRECT IMAGE
                    Quantity = model.Quantity
                });
            }

            HttpContext.Session.SetString("Cart",
                JsonConvert.SerializeObject(cart));

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


        public IActionResult Cart()
        {
            var cartJson = HttpContext.Session.GetString("Cart");

            List<CartItem> cart = new List<CartItem>();

            if (!string.IsNullOrEmpty(cartJson))
            {
                cart = JsonConvert.DeserializeObject<List<CartItem>>(cartJson);
            }

            return View(cart);
        }
        [HttpPost]
        public IActionResult UpdateCartQty([FromBody] CartItem model)
        {
            var cartJson = HttpContext.Session.GetString("Cart");

            if (string.IsNullOrEmpty(cartJson))
                return Json(new { success = false });

            var cart = JsonConvert.DeserializeObject<List<CartItem>>(cartJson);

            var item = cart.FirstOrDefault(x => x.VariantId == model.VariantId);

            if (item == null)
                return Json(new { success = false });

            item.Quantity += model.Change;

            // ? Prevent less than 1
            if (item.Quantity <= 0)
                cart.Remove(item);

            HttpContext.Session.SetString("Cart",
                JsonConvert.SerializeObject(cart));

            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult RemoveCartItem([FromBody] CartItem model)
        {
            var cartJson = HttpContext.Session.GetString("Cart");

            if (string.IsNullOrEmpty(cartJson))
                return Json(new { success = false });

            var cart = JsonConvert.DeserializeObject<List<CartItem>>(cartJson);

            var item = cart.FirstOrDefault(x => x.VariantId == model.VariantId);

            if (item != null)
                cart.Remove(item);

            HttpContext.Session.SetString("Cart",
                JsonConvert.SerializeObject(cart));

            return Json(new { success = true });
        }
        public IActionResult ReviewOrder()
        {
            var cartJson = HttpContext.Session.GetString("Cart");

            if (string.IsNullOrEmpty(cartJson))
                return RedirectToAction("Cart");

            var cart = JsonConvert.DeserializeObject<List<CartItem>>(cartJson);

            // Get logged in user
            string userIdStr = HttpContext.Session.GetString("UserId");
            int userId = Convert.ToInt32(userIdStr);

            // Get Address from DB
            var address = _context.Customers
                .Where(a => a.CustomerId == userId)
                .FirstOrDefault();

            var vm = new ReviewOrderVM
            {
                CartItems = cart,
                Address = address
            };

            return View(vm);
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
