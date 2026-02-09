using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MIEL.web.Models.EntityModels;
using MIEL.web.Models.ViewModel;
using MIEL.web.Data;
using System;
using System.Linq;

namespace MIEL.web.Controllers
{
    public class PurchaseController : Controller
    {
        private readonly AppDBContext _context;

        public PurchaseController(AppDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult SearchProducts(string term)
        {
            var products = _context.ProductMasters
                .Where(p => p.ProductName.Contains(term))
                .Select(p => new
                {
                    id = p.ProductId,
                    text = p.ProductName
                })
                .Take(20)
                .ToList();

            return Json(products);
        }

        // ============================
        // GET : CREATE PURCHASE
        // ============================
        public IActionResult Create()
        {
            var model = new PurchaseVM
            {
                PurchaseCode = GeneratePurchaseCode(),
                BatchNo = GenerateBatchNo(),
                PurchaseDate = DateTime.Today,
                Suppliers = _context.Suppliers
                    .Where(x => x.Status == "Active")
                    .Select(x => new SelectListItem
                    {
                        Value = x.SupplierId.ToString(),
                        Text = x.Name
                    }).ToList()
            };

            return View(model);
        }

        // ============================
        // POST : CREATE PURCHASE
        // ============================
        [HttpPost]
        public IActionResult Create(PurchaseVM model)
        {
            if (!ModelState.IsValid)
            {
                model.Suppliers = _context.Suppliers
                    .Select(x => new SelectListItem
                    {
                        Value = x.SupplierId.ToString(),
                        Text = x.Name
                    }).ToList();

                return View(model);
            }

            // ----------------------------
            // 1️⃣ PURCHASE MASTER
            // ----------------------------
            var purchase = new PurchaseMaster
            {
                SupplierId = model.SupplierId,
                InvoiceNo = model.PurchaseCode,
                InvoiceDate = model.PurchaseDate,
                GrandTotal = model.CostPrice * model.Quantity
            };

            _context.PurchaseMasters.Add(purchase);
            _context.SaveChanges();

            // ----------------------------
            // 2️⃣ FIND OR CREATE VARIANT
            // ----------------------------
            var variant = _context.ProColorSizeVariants.FirstOrDefault(x =>
                x.ProductId == model.ProductId &&
                x.colour == model.Color &&
                x.size == model.Size);

            if (variant == null)
            {
                variant = new procolrsizevarnt
                {
                    ProductId = model.ProductId,
                    colour = model.Color,
                    size = model.Size,
                    varientCode = model.VariantCode,
                    QuantityOnHand = 0,
                    AverageCost = model.CostPrice
                };

                _context.ProColorSizeVariants.Add(variant);
                _context.SaveChanges();
            }

            // ----------------------------
            // 3️⃣ PURCHASE ITEM
            // ----------------------------
            var item = new PurchaseItem
            {
                PurchaseId = purchase.PurchaseId,
                varientid = variant.varientid,
                Quantity = model.Quantity,
                Rate = model.CostPrice,
                BatchNo = model.BatchNo,
                TaxableAmount = model.Quantity * model.CostPrice,
                NetAmount = model.Quantity * model.CostPrice
            };

            _context.PurchaseItems.Add(item);

            // ----------------------------
            // 4️⃣ INVENTORY BATCH
            // ----------------------------
            var batch = new InventoryBatch
            {
                varientid = variant.varientid,
                BatchNo = model.BatchNo,
                QuantityIn = model.Quantity,
                QuantityOut = 0,
                CostPrice = model.CostPrice,
                CreatedDate = DateTime.Now
            };

            _context.InventoryBatch.Add(batch);

            // ----------------------------
            // 5️⃣ UPDATE STOCK
            // ----------------------------
            variant.QuantityOnHand += model.Quantity;
            variant.AverageCost = model.CostPrice;

            _context.SaveChanges();

            return RedirectToAction("Create");
        }

        // ============================
        // AUTO CODE GENERATORS
        // ============================
        private string GeneratePurchaseCode()
        {
            return "PUR-" + DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        private string GenerateBatchNo()
        {
            return "BAT-" + DateTime.Now.ToString("yyyyMMddHHmm");
        }
    }
}
