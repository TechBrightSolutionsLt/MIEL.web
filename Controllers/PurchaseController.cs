using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MIEL.web.Data;
using MIEL.web.Models.EntityModels;
using MIEL.web.Models.ViewModel;
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

        // ===========================
        // PRODUCT SEARCH (SELECT2)
        // ===========================
        [HttpGet]
        public IActionResult SearchProducts(string term)
        {
            var data = _context.ProductMasters
                .Where(x => x.ProductName.Contains(term))
                .Select(x => new { id = x.ProductId, text = x.ProductName })
                .Take(20)
                .ToList();

            return Json(data);
        }

        // ===========================
        // GET CREATE
        // ===========================
        public IActionResult Create()
        {
            var vm = new PurchaseVM
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

            return View(vm);
        }

        // ===========================
        // POST CREATE (SAVE ALL)
        // ===========================


        [HttpPost]
        public IActionResult Create(PurchaseVM model)
        {
            if (model.Items == null || !model.Items.Any())
            {
                ModelState.AddModelError("", "Add at least one item");
            }

            if (!ModelState.IsValid)
            {
                model.Suppliers = _context.Suppliers
                    .Where(x => x.Status == "Active")
                    .Select(x => new SelectListItem
                    {
                        Value = x.SupplierId.ToString(),
                        Text = x.Name
                    }).ToList();

                return View(model);
            }

            // ===========================
            // 1️⃣ PURCHASE MASTER
            // ===========================
            var purchase = new PurchaseMaster
            {
                SupplierId = model.SupplierId,
                InvoiceNo = model.PurchaseCode,
                InvoiceDate = model.PurchaseDate,
                TotalDisc = model.Items.Sum(x => x.DiscAmount),
                TotalTax = model.Items.Sum(x => x.GstAmount),
                TotalTaxable = model.Items.Sum(x => (x.Rate * x.Quantity) - x.DiscAmount),
                GrandTotal = model.Items.Sum(x => x.Amount)
            };

            _context.PurchaseMasters.Add(purchase);
            _context.SaveChanges(); // get PurchaseId

            // ===========================
            // 2️⃣ FIND OR CREATE VARIANT (ONLY ONCE)
            // ===========================
            var variant = _context.ProColorSizeVariants
                .FirstOrDefault(x =>
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
                    AverageCost = model.Items.First().Rate
                };

                _context.ProColorSizeVariants.Add(variant);
                _context.SaveChanges(); // get varientid
            }

            // ===========================
            // 3️⃣ LOOP ITEMS
            // ===========================
            foreach (var item in model.Items)
            {
                var pItem = new PurchaseItem
                {
                    PurchaseId = purchase.PurchaseId,
                    varientid = variant.varientid,
                    Quantity = item.Quantity,
                    Rate = item.Rate,
                    BatchNo = model.BatchNo,
                    GstPercent = item.GstPercent,
                    GstAmount = item.GstAmount,
                    DiscPercent = item.DiscPercent,
                    DiscAmount = item.DiscAmount,
                    TaxableAmount = (item.Rate * item.Quantity) - item.DiscAmount,
                    NetAmount = item.Amount
                };

                _context.PurchaseItems.Add(pItem);

                // Inventory batch
                var batch = new InventoryBatch
                {
                    varientid = variant.varientid,
                    BatchNo = model.BatchNo,
                    QuantityIn = item.Quantity,
                    QuantityOut = 0,
                    CostPrice = item.Rate,
                    CreatedDate = DateTime.Now,
                    SellingPrice = item.SellingPrice
                };

                _context.InventoryBatch.Add(batch);

                // Update stock
                variant.QuantityOnHand += item.Quantity;
                variant.AverageCost = item.Rate;

                // Update variant price
                var oldPrices = _context.VariantPrices
                    .Where(x => x.varientid == variant.varientid && x.IsActive)
                    .ToList();

                foreach (var price in oldPrices)
                {
                    price.IsActive = false;
                }

                _context.VariantPrices.Add(new VariantPrice
                {
                    varientid = variant.varientid,
                    SellingPrice = item.SellingPrice,
                    IsActive = true
                });
            }

            _context.SaveChanges();

            return RedirectToAction("Create");
        }




        //[HttpPost]
        //public IActionResult Create(PurchaseVM model)
        //{
        //    if (model.Items == null || !model.Items.Any())
        //    {
        //        ModelState.AddModelError("", "Add at least one item");
        //    }

        //    if (!ModelState.IsValid)
        //    {
        //        model.Suppliers = _context.Suppliers
        //            .Select(x => new SelectListItem
        //            {
        //                Value = x.SupplierId.ToString(),
        //                Text = x.Name
        //            }).ToList();
        //        return View(model);
        //    }

        //    // ===========================
        //    // 1️⃣ PURCHASE MASTER
        //    // ===========================
        //    var purchase = new PurchaseMaster
        //    {
        //        SupplierId = model.SupplierId,
        //        InvoiceNo = model.PurchaseCode,
        //        InvoiceDate = model.PurchaseDate,
        //        TotalDisc = model.Items.Sum(x => x.DiscAmount),
        //        TotalTax = model.Items.Sum(x => x.GstAmount),
        //        TotalTaxable = model.Items.Sum(x => (x.Rate * x.Quantity) - x.DiscAmount),
        //        GrandTotal = model.Items.Sum(x => x.Amount)
        //    };

        //    _context.PurchaseMasters.Add(purchase);
        //    _context.SaveChanges(); // GET PurchaseId

        //    // ===========================
        //    // 2️⃣ LOOP ITEMS
        //    // ===========================
        //    foreach (var item in model.Items)
        //    {
        //        // ---------------------------
        //        // VARIANT (FIND OR CREATE)
        //        // ---------------------------
        //        var variant = _context.ProColorSizeVariants
        //            .FirstOrDefault(x => x.varientCode == item.VariantCode);

        //        if (variant == null)
        //        {
        //            variant = new procolrsizevarnt
        //            {
        //                ProductId = model.ProductId,
        //                colour = model.Color,
        //                size = model.Size,
        //                varientCode = item.VariantCode,
        //                QuantityOnHand = 0,
        //                AverageCost = item.Rate
        //            };

        //            _context.ProColorSizeVariants.Add(variant);
        //            _context.SaveChanges();
        //        }

        //        // ---------------------------
        //        // PURCHASE ITEM
        //        // ---------------------------
        //        var pItem = new PurchaseItem
        //        {
        //            PurchaseId = purchase.PurchaseId,
        //            varientid = variant.varientid,
        //            Quantity = item.Quantity,
        //            Rate = item.Rate,
        //            BatchNo = model.BatchNo,
        //            GstPercent = item.GstPercent,
        //            GstAmount = item.GstAmount,
        //            DiscPercent = item.DiscPercent,
        //            DiscAmount = item.DiscAmount,
        //            TaxableAmount = (item.Rate * item.Quantity) - item.DiscAmount,
        //            NetAmount = item.Amount
        //        };

        //        _context.PurchaseItems.Add(pItem);

        //        // ---------------------------
        //        // INVENTORY BATCH
        //        // ---------------------------
        //        var batch = new InventoryBatch
        //        {
        //            varientid = variant.varientid,
        //            BatchNo = model.BatchNo,
        //            QuantityIn = item.Quantity,
        //            QuantityOut = 0,
        //            CostPrice = item.Rate,
        //            CreatedDate = DateTime.Now
        //        };

        //        _context.InventoryBatch.Add(batch);

        //        // ---------------------------
        //        // UPDATE STOCK
        //        // ---------------------------
        //        variant.QuantityOnHand += item.Quantity;
        //        variant.AverageCost = item.Rate;

        //        // ---------------------------
        //        // VARIANT PRICE (OPTIONAL)
        //        // ---------------------------
        //        var oldPrices = _context.VariantPrices
        //         .Where(x => x.varientid == variant.varientid && x.IsActive)
        //        .ToList();

        //        foreach (var price in oldPrices)
        //        {
        //            price.IsActive = false;
        //        }




        //        if (!_context.VariantPrices.Any(x => x.varientid == variant.varientid))
        //        {
        //            _context.VariantPrices.Add(new VariantPrice
        //            {
        //                varientid = variant.varientid,
        //                SellingPrice = item.SellingPrice,
        //                IsActive = true
        //            });

        //        }
        //    }

        //    _context.SaveChanges();

        //    return RedirectToAction("Create");
        //}

        // ===========================
        // AUTO CODES
        // ===========================
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
