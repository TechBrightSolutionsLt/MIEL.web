using Microsoft.AspNetCore.Mvc;
using MIEL.web.Data;
using MIEL.web.Models.ViewModel;
using System;
using System.Linq;

namespace MIEL.web.Controllers
{
    public class SalesController : Controller
    {
        private readonly AppDBContext _db;

        public SalesController(AppDBContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Create()
        {
            var vm = new SaleCreateVM
            {
                SaleCode = GenerateSaleCode(),
                SaleDate = DateTime.Today,
                Products = _db.ProductMasters.ToList()
            };

            return View(vm);
        }
        private string GenerateSaleCode()
        {
            return $"SAL-{DateTime.Now:yyyyMMdd-HHmm}";
        }
        [HttpGet]
        public IActionResult SearchProducts(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Json(new List<object>());

            var products = _db.ProductMasters
                .Where(p => p.ProductName.Contains(term))
                .Select(p => new
                {
                    productId = p.ProductId,
                    productName = p.ProductName
                })
                .Take(10)
                .ToList();

            return Json(products);
        }
        





        [HttpGet]
        public IActionResult SearchCustomer(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Json(new List<object>());

            var customers = _db.Customers
                .Where(c => c.Name.Contains(term))
                .Select(c => new
                {
                    customerId = c.CustomerId,
                    name = c.Name,
                    mobile = c.Mobile   // 👈 IMPORTANT
                })
                .Take(10)
                .ToList();

            return Json(customers);
        }

        //private string GenerateSaleCode()
        //{
        //    var today = DateTime.Today;

        //    var todayCount = _db.Sales
        //        .Count(s => s.SaleDate.Date == today) + 1;

        //    return $"SAL-{today:yyyyMMdd}-{todayCount:D3}";
        //}
    }
}