using Microsoft.AspNetCore.Mvc;
using MIEL.web.Data;
using MIEL.web.Models;
using MIEL.web.Models.EntityModels;
using MIEL.web.Models.ViewModel;
using Newtonsoft.Json;
using System.Diagnostics;

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
            string customerId = HttpContext.Session.GetString("CustomerId");
            string guestId = GetGuestId();

            // PICK VARIANT
            var variant = _context.ProColorSizeVariants
                .Where(v => v.ProductId == model.ProductId
                            && v.colour == model.Color
                            && v.size == model.Size)
                .Select(v => new
                {
                    v.varientid
                })
                .FirstOrDefault();

            if (variant == null)
                return Json(new { success = false });

            // GET RATE separately
            var rate = _context.PurchaseItems
                .Where(pi => pi.varientid == variant.varientid)
                .OrderByDescending(pi => pi.PurchaseItemId)
                .Select(pi => pi.Rate)
                .FirstOrDefault();

            // GET IMAGE AND NAME
            var image = _context.ProductImages
                .Where(i => i.ProductId == model.ProductId && i.Flag == 1)
                .Select(i => i.ImgPath)
                .FirstOrDefault();

            var productName = _context.ProductMasters
                .Where(p => p.ProductId == model.ProductId)
                .Select(p => p.ProductName)
                .FirstOrDefault();

            // CHECK EXISTING ITEM
            Cart existingItem = null;
            if (!string.IsNullOrEmpty(customerId))
            {
                int custId = Convert.ToInt32(customerId);
                existingItem = _context.Cart.FirstOrDefault(x =>
                    x.CustomerId == custId &&
                    x.VariantId == variant.varientid);
            }
            else
            {
                existingItem = _context.Cart.FirstOrDefault(x =>
                    x.GuestId == guestId &&
                    x.VariantId == variant.varientid);
            }

            // UPDATE OR INSERT
            if (existingItem != null)
                existingItem.Quantity += model.Quantity;
            else
            {
                Cart newItem = new Cart
                {
                    CustomerId = string.IsNullOrEmpty(customerId) ? (int?)null : Convert.ToInt32(customerId),
                    GuestId = string.IsNullOrEmpty(customerId) ? guestId : null,
                    ProductId = model.ProductId,
                    VariantId = variant.varientid,
                    ProductName = productName,
                    Color = model.Color,
                    Size = model.Size,
                    Price = rate,
                    Image = image,
                    Quantity = model.Quantity,
                    CreatedDate = DateTime.Now
                };
                _context.Cart.Add(newItem);
            }

            _context.SaveChanges();

            // RETURN COUNT
            int count = !string.IsNullOrEmpty(customerId)
                ? _context.Cart.Count(x => x.CustomerId == Convert.ToInt32(customerId))
                : _context.Cart.Count(x => x.GuestId == guestId);

            return Json(new { success = true, count });
        }




        [HttpPost]
        public IActionResult AddToCartFromCategory(int productId)
        {
            string customerId = HttpContext.Session.GetString("CustomerId");
            string guestId = GetGuestId();

            // pick first variant of product
            var variant = _context.ProColorSizeVariants
                .Where(v => v.ProductId == productId)
                .OrderBy(v => v.varientid) // pick first
                .Select(v => new { v.varientid, v.colour, v.size })
                .FirstOrDefault();

            if (variant == null)
                return Json(new { success = false });

            // get latest rate
            var rate = _context.PurchaseItems
                .Where(pi => pi.varientid == variant.varientid)
                .OrderByDescending(pi => pi.PurchaseItemId)
                .Select(pi => pi.Rate)
                .FirstOrDefault();

            // image & name
            var image = _context.ProductImages
                .Where(i => i.ProductId == productId && i.Flag == 1)
                .Select(i => i.ImgPath)
                .FirstOrDefault();

            var productName = _context.ProductMasters
                .Where(p => p.ProductId == productId)
                .Select(p => p.ProductName)
                .FirstOrDefault();

            // check existing item
            Cart existingItem = null;
            if (!string.IsNullOrEmpty(customerId))
            {
                int custId = Convert.ToInt32(customerId);
                existingItem = _context.Cart.FirstOrDefault(x =>
                    x.CustomerId == custId && x.VariantId == variant.varientid);
            }
            else
            {
                existingItem = _context.Cart.FirstOrDefault(x =>
                    x.GuestId == guestId && x.VariantId == variant.varientid);
            }

            // insert or update
            if (existingItem != null)
                existingItem.Quantity += 1;
            else
            {
                _context.Cart.Add(new Cart
                {
                    CustomerId = string.IsNullOrEmpty(customerId) ? (int?)null : Convert.ToInt32(customerId),
                    GuestId = string.IsNullOrEmpty(customerId) ? guestId : null,
                    ProductId = productId,
                    VariantId = variant.varientid,
                    ProductName = productName,
                    Color = variant.colour,
                    Size = variant.size,
                    Price = rate,
                    Image = image,
                    Quantity = 1,
                    CreatedDate = DateTime.Now
                });
            }

            _context.SaveChanges();

            int count = !string.IsNullOrEmpty(customerId)
                ? _context.Cart.Count(x => x.CustomerId == Convert.ToInt32(customerId))
                : _context.Cart.Count(x => x.GuestId == guestId);

            return Json(new { success = true, count });
        }



        [HttpGet]
        public IActionResult GetCartCount()
        {
            string customerId = HttpContext.Session.GetString("CustomerId");
            string guestId = GetGuestId();


            int count = 0;

            // Logged user → count by CustomerId
            if (!string.IsNullOrEmpty(customerId))
            {
                int cid = Convert.ToInt32(customerId);

                count = _context.Cart
                    .Where(c => c.CustomerId == cid)
                    .Count();
            }

            // Guest user → count by GuestId
            else if (!string.IsNullOrEmpty(guestId))
            {
                count = _context.Cart
                    .Where(c => c.GuestId == guestId)
                    .Count();
            }

            return Json(new { count });
        }

        private string GetGuestId()
        {
            string guestId = Request.Cookies["GuestId"];

            if (string.IsNullOrEmpty(guestId))
            {
                guestId = Guid.NewGuid().ToString();

                Response.Cookies.Append("GuestId", guestId, new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(30), // persists 30 days
                    HttpOnly = true,
                    IsEssential = true
                });
            }

            return guestId;
        }


        public IActionResult Cart()
        {
            string customerId = HttpContext.Session.GetString("CustomerId");
            string guestId = GetGuestId();


            List<CartItem> cartItems = new List<CartItem>();

            // Logged user
            if (!string.IsNullOrEmpty(customerId))
            {
                int cid = Convert.ToInt32(customerId);

                cartItems = _context.Cart
                    .Where(c => c.CustomerId == cid)
                    .Select(c => new CartItem
                    {
                        ProductId = c.ProductId,
                        VariantId = c.VariantId,
                        ProductName = c.ProductName,
                        Color = c.Color,
                        Size = c.Size,
                        Price = c.Price,
                        Quantity = c.Quantity,
                        Image = c.Image
                    })
                    .ToList();
            }
            // Guest user
            else if (!string.IsNullOrEmpty(guestId))
            {
                cartItems = _context.Cart
                    .Where(c => c.GuestId == guestId)
                    .Select(c => new CartItem
                    {
                        ProductId = c.ProductId,
                        VariantId = c.VariantId,
                        ProductName = c.ProductName,
                        Color = c.Color,
                        Size = c.Size,
                        Price = c.Price,
                        Quantity = c.Quantity,
                        Image = c.Image
                    })
                    .ToList();
            }

            return View(cartItems);
        }


[HttpPost]
public IActionResult UpdateCartQty([FromBody] CartItem model)
{
    string customerId = HttpContext.Session.GetString("CustomerId");
    string guestId = GetGuestId();

    Cart item = null;

    if (!string.IsNullOrEmpty(customerId))
    {
        int cid = Convert.ToInt32(customerId);
        item = _context.Cart.FirstOrDefault(x =>
            x.CustomerId == cid &&
            x.VariantId == model.VariantId);
    }
    else if (!string.IsNullOrEmpty(guestId))
    {
        item = _context.Cart.FirstOrDefault(x =>
            x.GuestId == guestId &&
            x.VariantId == model.VariantId);
    }

    if (item == null)
        return Json(new { success = false });

    int newQty = item.Quantity + model.Change;

    if (newQty < 0)
        newQty = 0;

    // 🔥 Get stock
    var variant = _context.ProColorSizeVariants
        .FirstOrDefault(x => x.varientid == model.VariantId);

    if (variant == null)
        return Json(new { success = false, message = "Variant not found" });

    // 🚨 STOCK CHECK ONLY (NO UPDATE)
    if (variant.QuantityOnHand == 0)
    {
        return Json(new
        {
            success = false,
            message = "Out of stock"
        });
    }

    if (newQty > variant.QuantityOnHand)
    {
        return Json(new
        {
            success = false,
            message = "Only " + variant.QuantityOnHand + " items available"
        });
    }

    // ✅ Update cart only
    if (newQty == 0)
    {
        _context.Cart.Remove(item);
    }
    else
    {
        item.Quantity = newQty;
    }

    _context.SaveChanges();

    return Json(new { success = true });
}




        [HttpPost]
        public IActionResult RemoveCartItem([FromBody] CartItem model)
        {
            string customerIdStr = HttpContext.Session.GetString("CustomerId");
            string guestId = Request.Cookies["GuestId"];

            Cart item = null;

            // Logged user
            if (!string.IsNullOrEmpty(customerIdStr))
            {
                int customerId = Convert.ToInt32(customerIdStr);

                item = _context.Cart.FirstOrDefault(x =>
                    x.CustomerId.HasValue &&
                    x.CustomerId.Value == customerId &&
                    x.VariantId == model.VariantId);
            }
            // Guest user
            else if (!string.IsNullOrEmpty(guestId))
            {
                item = _context.Cart.FirstOrDefault(x =>
                    x.GuestId != null &&
                    x.GuestId == guestId &&
                    x.VariantId == model.VariantId);
            }

            if (item == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Item not found"
                });
            }

            _context.Cart.Remove(item);
            _context.SaveChanges();

            int count = 0;

            if (!string.IsNullOrEmpty(customerIdStr))
            {
                int customerId = Convert.ToInt32(customerIdStr);

                count = _context.Cart
                    .Where(x => x.CustomerId.HasValue && x.CustomerId.Value == customerId)
                    .Count();
            }
            else if (!string.IsNullOrEmpty(guestId))
            {
                count = _context.Cart
                    .Where(x => x.GuestId != null && x.GuestId == guestId)
                    .Count();
            }

            return Json(new
            {
                success = true,
                count = count
            });
        }



        public IActionResult ReviewOrder()
        {
            string userIdStr = HttpContext.Session.GetString("CustomerId");

            // Checkout requires login
            if (string.IsNullOrEmpty(userIdStr))
                return RedirectToAction("IndexLogin", "Login");

            int customerId = Convert.ToInt32(userIdStr);

            var cart = _context.Cart
                .Where(c => c.CustomerId == customerId)
                .Select(c => new CartItem
                {
                    ProductId = c.ProductId,
                    VariantId = c.VariantId,
                    ProductName = c.ProductName,
                    Color = c.Color,
                    Size = c.Size,
                    Price = c.Price,
                    Quantity = c.Quantity,
                    Image = c.Image
                })
                .ToList();

            if (cart == null || !cart.Any())
                return RedirectToAction("Cart", "Cart");




            int? existingSalesId = HttpContext.Session.GetInt32("SalesId");
            SalesMaster salesMasters = null;

            if (existingSalesId != null)
            {
                // Check if SalesMaster exists in database
                salesMasters = _context.SalesMasters
                    .FirstOrDefault(x => x.SalesId == existingSalesId.Value);
            }


            if (existingSalesId == null)
            {
                decimal totalAmount = cart.Sum(x => x.Price * x.Quantity);
                decimal discount = 0;

                var salesMaster = new SalesMaster
                {
                    SalesDate = DateTime.Now,
                    InvoiceNo = "SAL-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                    PaymentType = "Pending",
                    CustomerId = customerId,
                    TotalAmount = totalAmount,
                    TotalDiscount = discount,
                    NetAmount = totalAmount,
                    paysts = 0,
                    salesmode = 2
                };

                _context.SalesMasters.Add(salesMaster);
                _context.SaveChanges();

                // Save SalesId in Session
                HttpContext.Session.SetInt32("SalesId", salesMaster.SalesId);

                // Assign to salesMasters so it’s not null
                salesMasters = salesMaster;

                foreach (var item in cart)
                {
                    var batch = _context.InventoryBatch
        .Where(b => b.varientid == item.VariantId && (b.QuantityIn - b.QuantityOut) > 0)
        .OrderBy(b => b.CreatedDate) // FIFO: earliest created batch first
        .FirstOrDefault();

                    string batchNo = batch != null ? batch.BatchNo : "";
                    var salesItem = new SalesItem
                    {
                        SalesId = salesMaster.SalesId,
                        varientid = item.VariantId,
                        BatchNo = batchNo,
                        Quantity = item.Quantity,
                        SellingPrice = item.Price,
                        DiscPercent = 0,
                        DiscAmount = 0,
                        TaxAmount = 0,
                        NetAmount = item.Price * item.Quantity
                    };

                    _context.SalesItems.Add(salesItem);
                }

                _context.SaveChanges();
            }


            var address = _context.users_TB
                .Where(a => a.CustomerId == customerId)
                .Select(a => new Customer
                {
                    CustomerId = a.CustomerId,
                    Name = (a.FirstName ?? "") + " " + (a.LastName ?? ""),
                    BuildingName = a.Address,
                    City = a.City,
                    Pin = a.Postcode,
                    Mobile = a.MobileNumber
                })
                .FirstOrDefault();

            var paymentVM = new PaymentVM
            {
                SalesId = salesMasters.SalesId,
                InvoiceNo = salesMasters.InvoiceNo,
                TotalAmount = salesMasters.TotalAmount,
                Items = cart
            };

            return View("Payment", paymentVM);
        }

        [HttpPost]
        public IActionResult ConfirmOrder()
        {
            int? salesId = HttpContext.Session.GetInt32("SalesId");

            if (salesId == null)
            {
                return RedirectToAction("Cart", "Cart");
            }

            // Redirect to payment page
            return RedirectToAction("Payment", "Home", new { salesId = salesId });
        }
        public IActionResult Payment(int salesId)
        {
            var order = _context.SalesMasters
                .Where(x => x.SalesId == salesId)
                .Select(x => new PaymentVM
                {
                    SalesId = x.SalesId,
                    InvoiceNo = x.InvoiceNo,
                    TotalAmount = x.NetAmount,
                    PayStatus = x.paysts,

                    // LOAD ITEMS
                    Items = _context.Cart
                        .Where(c => c.CustomerId == x.CustomerId)
                        .Select(c => new CartItem
                        {
                            ProductName = c.ProductName,
                            Quantity = c.Quantity,
                            Price = c.Price,
                            Image = c.Image
                        }).ToList()
                })
                .FirstOrDefault();

            if (order == null)
                return RedirectToAction("Cart", "Cart");

            return View(order);
        }



        [HttpGet]
     
        public IActionResult ConfirmCOD(int salesId)
        {
            var sales = _context.SalesMasters
                .FirstOrDefault(x => x.SalesId == salesId);

            if (sales == null)
                return RedirectToAction("Cart", "Cart");

            sales.PaymentType = "Cash";
            sales.paysts = 0;

            _context.SaveChanges();

            HttpContext.Session.Remove("SalesId");

            var order = new OrderVM
            {
                CustomerId = sales.CustomerId,
                SalesId = sales.SalesId, // <-- Save the SalesMaster ID here
                TotalAmount = sales.TotalAmount,
                OrderNumber = "ORD" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                PaymentStatus = "NotPaid",
                VerifyId = 0
            };

            _context.Orders.Add(order);
            _context.SaveChanges();

            // ✅ Clear customer's cart
            ClearCustomerCart(sales.CustomerId);

            return RedirectToAction("OrderSuccess", new { salesId = salesId });
        }


        private void ClearCustomerCart(int customerId)
        {
            var cartItems = _context.Cart
                .Where(c => c.CustomerId == customerId)
                .ToList();

            if (cartItems.Any())
            {
                _context.Cart.RemoveRange(cartItems);
                _context.SaveChanges();
            }
        }
        public IActionResult OrderSuccess(int salesId)
        {
            var order = _context.Orders
                .FirstOrDefault(x => x.SalesId == salesId);

            if (order == null)
                return RedirectToAction("Cart", "Cart");

            return View(order);
        }


        [HttpGet]
        public IActionResult ConfirmPayID(int salesId)
        {
            var sales = _context.SalesMasters
                .FirstOrDefault(x => x.SalesId == salesId);

            if (sales == null)
                return RedirectToAction("Cart", "Cart");

            // Mark as PayID payment type (pending confirmation)
            sales.PaymentType = "PayID";
            sales.paysts = 0; // still pending
            _context.SaveChanges();

            // ✅ Create an Order entry immediately
            var orderNumber = "ORD" + DateTime.Now.ToString("yyyyMMddHHmmss");

            var order = new OrderVM
            {
                CustomerId = sales.CustomerId,
                SalesId = sales.SalesId,
                TotalAmount = sales.TotalAmount,
                OrderNumber = orderNumber,
                PaymentStatus = "NotPaid",
                VerifyId = 0
            };

            _context.Orders.Add(order);
            _context.SaveChanges();

            // Clear customer's cart/session if you want
            // ClearCustomerCart(sales.CustomerId); 
            // HttpContext.Session.Remove("SalesId");

            // Prepare ViewModel to show on PayIDPage
            var vm = new PayIDViewModel
            {
                SalesId = sales.SalesId,
                OrderId = order.Id,               // <-- Order ID saved
                OrderNumber = order.OrderNumber,  // <-- show this as invoice
                TotalAmount = sales.TotalAmount,
                PayId = 0430823457,               // static PayID
                Email = "binoyjoseph@y7mail.com"
            };
            ClearCustomerCart(sales.CustomerId);
            return View("PayIDPage", vm);
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
