using Microsoft.AspNetCore.Mvc;
using MIEL.web.Data;
using MIEL.web.Models.ViewModel;
using System;
using System.Linq;
using System.Collections.Generic;

namespace MIEL.web.Controllers
{
    public class PurchaseController : Controller
    {
        private readonly AppDBContext _db;

        public PurchaseController(AppDBContext db)
        {
            _db = db;
        }

        // ===============================
        // CREATE PURCHASE
        // ===============================
        [HttpGet]
        public IActionResult Create()
        {
            var vm = new PurchaseVM
            {
                PurchaseCode = GeneratePurchaseCode(),
                PurchaseDate = DateTime.Today,
                Products = _db.ProductMasters.ToList()
            };

            return View(vm);
        }

        // ===============================
        // GENERATE PURCHASE CODE
        // ===============================
        private string GeneratePurchaseCode()
        {
            return $"PUR-{DateTime.Now:yyyyMMdd-HHmm}";
        }

        // ===============================
        // PRODUCT AUTOCOMPLETE
        // ===============================
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

        // ===============================
        // SUPPLIER AUTOCOMPLETE
        // ===============================
        [HttpGet]
        public IActionResult SearchSupplier(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Json(new List<object>());

            var suppliers = _db.Suppliers
                .Where(s => s.Name.Contains(term))
                .Select(s => new
                {
                    supplierId = s.SupplierId,
                    name = s.Name,
                    phone = s.Phone
                })
                .Take(10)
                .ToList();

            return Json(suppliers);
        }

        // ===============================
        // SAVE PURCHASE (POST)
        // ===============================
        [HttpPost]
        public IActionResult Create(PurchaseVM model, string action)
        {
            if (!ModelState.IsValid)
            {
                model.Products = _db.ProductMasters.ToList();
                return View(model);
            }

            // 🔹 Purchase save logic will go here
            // 1. Save Purchase header
            // 2. Save Purchase items
            // 3. Update stock quantities
            // 4. Mark status (Draft / Completed)

            return RedirectToAction("Index");
        }
    }
}