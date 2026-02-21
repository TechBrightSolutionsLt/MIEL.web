using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MIEL.web.Data;
using MIEL.web.Models.EntityModels;
using MIEL.web.Models.ViewModel;
using System;
using System.Linq;
using Newtonsoft.Json;

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
                .Select(x => new { id = x.ProductId, text = x.ProductName , code = x.ProductCode })
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
            try
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
                // 1️⃣ SAVE PURCHASE MASTER
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
                _context.SaveChanges();

                // ===========================
                // 2️⃣ LOOP ITEMS
                // ===========================
                for (int i = 0; i < model.Items.Count; i++)
                {
                    var item = model.Items[i];

                    // 🔹 Read hidden fields manually
                    int productId = Convert.ToInt32(Request.Form[$"ProductId_{i}"]);
                    string color = Request.Form[$"Color_{i}"];
                    string size = Request.Form[$"Size_{i}"];

                    // 🔍 FIND VARIANT
                    var variant = _context.ProColorSizeVariants
                        .FirstOrDefault(x => x.varientCode == item.VariantCode);

                    // If not found → create new variant
                    if (variant == null)
                    {
                        variant = new procolrsizevarnt
                        {
                            ProductId = productId,
                            colour = color,
                            size = size,
                            varientCode = item.VariantCode,
                            QuantityOnHand = 0,
                            AverageCost = item.Rate
                        };

                        _context.ProColorSizeVariants.Add(variant);
                        _context.SaveChanges();
                    }

                    // ===========================
                    // SAVE PURCHASE ITEM
                    // ===========================
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

                    // ===========================
                    // INVENTORY BATCH
                    // ===========================
                    var batch = new InventoryBatch
                    {
                        varientid = variant.varientid,
                        BatchNo = model.BatchNo,
                        QuantityIn = item.Quantity,
                        QuantityOut = 0,
                        CostPrice = item.Rate,
                        SellingPrice = item.SellingPrice,
                        CreatedDate = DateTime.Now
                    };

                    _context.InventoryBatch.Add(batch);

                    // ===========================
                    // UPDATE STOCK
                    // ===========================
                    variant.QuantityOnHand += item.Quantity;
                    variant.AverageCost = item.Rate;

                    // ===========================
                    // UPDATE SELLING PRICE
                    // ===========================
                    var oldPrices = _context.VariantPrices
                        .Where(x => x.varientid == variant.varientid && x.IsActive)
                        .ToList();

                    foreach (var price in oldPrices)
                        price.IsActive = false;

                    _context.VariantPrices.Add(new VariantPrice
                    {
                        varientid = variant.varientid,
                        SellingPrice = item.SellingPrice,
                        IsActive = true
                    });
                }

                _context.SaveChanges();

                TempData["SuccessMessage"] = "Purchase Created Successfully";
                return RedirectToAction("Create");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error: " + ex.Message;

                model.Suppliers = _context.Suppliers
                    .Where(x => x.Status == "Active")
                    .Select(x => new SelectListItem
                    {
                        Value = x.SupplierId.ToString(),
                        Text = x.Name
                    }).ToList();

                return View(model);
            }
        }

        // ===============================
        // PURCHASE LIST
        // ===============================
        public IActionResult Index(string search)
        {
            var query = _context.PurchaseMasters
                .OrderByDescending(x => x.PurchaseId)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.InvoiceNo.Contains(search));
            }

            var list = query.Select(x => new PurchaseListVM
            {
                PurchaseId = x.PurchaseId,
                InvoiceNo = x.InvoiceNo,
                InvoiceDate = x.InvoiceDate,
                GrandTotal = x.GrandTotal,
                SupplierName = _context.Suppliers
                    .Where(s => s.SupplierId == x.SupplierId)
                    .Select(s => s.Name)
                    .FirstOrDefault()
            }).ToList();

            return View(list);
        }


        // ===============================
        // PURCHASE DELETE
        // ===============================
        public IActionResult Delete(int id)
        {
            var purchase = _context.PurchaseMasters
                .FirstOrDefault(x => x.PurchaseId == id);

            if (purchase == null)
                return RedirectToAction("Index");

            var items = _context.PurchaseItems
                .Where(x => x.PurchaseId == id)
                .ToList();

            foreach (var item in items)
            {
                var variant = _context.ProColorSizeVariants
                    .FirstOrDefault(v => v.varientid == item.varientid);

                if (variant != null)
                {
                    variant.QuantityOnHand -= item.Quantity;
                }

                var batches = _context.InventoryBatch
                    .Where(b => b.varientid == item.varientid && b.BatchNo == item.BatchNo)
                    .ToList();

                _context.InventoryBatch.RemoveRange(batches);
            }

            _context.PurchaseItems.RemoveRange(items);
            _context.PurchaseMasters.Remove(purchase);

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // ===============================
        // PURCHASE EDIT - GET
        // ===============================
        public IActionResult Edit(int id)
        {
            // Get purchase master
            var purchase = _context.PurchaseMasters
                .FirstOrDefault(x => x.PurchaseId == id);

            if (purchase == null)
                return RedirectToAction("Index");

            // Create ViewModel
            var vm = new PurchaseVM
            {
                SupplierId = purchase.SupplierId,
                PurchaseCode = purchase.InvoiceNo,
                PurchaseDate = purchase.InvoiceDate,
                Suppliers = _context.Suppliers
                    .Where(x => x.Status == "Active")
                    .Select(x => new SelectListItem
                    {
                        Value = x.SupplierId.ToString(),
                        Text = x.Name
                    }).ToList(),

                Items = new List<PurchaseItemVM>()  // IMPORTANT
            };

            // Load purchase items with JOIN (Best Practice)
            vm.Items = (from pi in _context.PurchaseItems
                        join v in _context.ProColorSizeVariants
                            on pi.varientid equals v.varientid
                        join p in _context.ProductMasters
                            on v.ProductId equals p.ProductId
                        join vp in _context.VariantPrices
                            on v.varientid equals vp.varientid into vpJoin
                        from vp in vpJoin.Where(x => x.IsActive).DefaultIfEmpty()
                        where pi.PurchaseId == id
                        select new PurchaseItemVM
                        {
                            ProductName = p.ProductName,
                            VariantCode = v.varientCode,
                            Rate = pi.Rate,
                            Quantity = pi.Quantity,
                            DiscPercent = pi.DiscPercent,
                            DiscAmount = pi.DiscAmount,
                            GstPercent = pi.GstPercent,
                            GstAmount = pi.GstAmount,
                            Amount = pi.NetAmount,
                            SellingPrice = vp != null ? vp.SellingPrice : 0
                        }).ToList();

            // Optional: Load BatchNo (if stored in item)
            vm.BatchNo = _context.PurchaseItems
                .Where(x => x.PurchaseId == id)
                .Select(x => x.BatchNo)
                .FirstOrDefault();

            ViewBag.PurchaseId = id;

            return View(vm);
        }




        // ===============================
        // PURCHASE edit post
        // ===============================
        [HttpPost]
        public IActionResult Edit(int id, PurchaseVM model)
        {
            try
            {
                var purchase = _context.PurchaseMasters
                    .FirstOrDefault(x => x.PurchaseId == id);

                if (purchase == null)
                    return RedirectToAction("Index");

                // ===========================
                // 1️⃣ GET OLD ITEMS
                // ===========================
                var oldItems = _context.PurchaseItems
                    .Where(x => x.PurchaseId == id)
                    .ToList();

                // ===========================
                // 2️⃣ REVERSE OLD STOCK
                // ===========================
                foreach (var item in oldItems)
                {
                    var variant = _context.ProColorSizeVariants
                        .FirstOrDefault(v => v.varientid == item.varientid);

                    if (variant != null)
                    {
                        variant.QuantityOnHand -= item.Quantity;
                    }

                    // remove correct batch
                    var batch = _context.InventoryBatch
                        .FirstOrDefault(b => b.varientid == item.varientid && b.BatchNo == item.BatchNo);

                    if (batch != null)
                        _context.InventoryBatch.Remove(batch);
                }

                _context.PurchaseItems.RemoveRange(oldItems);

                // ===========================
                // 3️⃣ UPDATE PURCHASE MASTER
                // ===========================
                purchase.SupplierId = model.SupplierId;
                purchase.InvoiceDate = model.PurchaseDate;
                purchase.TotalDisc = model.Items.Sum(x => x.DiscAmount);
                purchase.TotalTax = model.Items.Sum(x => x.GstAmount);
                purchase.TotalTaxable = model.Items.Sum(x => (x.Rate * x.Quantity) - x.DiscAmount);
                purchase.GrandTotal = model.Items.Sum(x => x.Amount);

                _context.SaveChanges();

                // ===========================
                // 4️⃣ ADD NEW ITEMS
                // ===========================
                foreach (var item in model.Items)
                {
                    var variant = _context.ProColorSizeVariants
                        .FirstOrDefault(x => x.varientCode == item.VariantCode);

                    if (variant == null)
                        continue;

                    // save purchase item
                    var newItem = new PurchaseItem
                    {
                        PurchaseId = id,
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

                    _context.PurchaseItems.Add(newItem);

                    // save inventory batch
                    var batch = new InventoryBatch
                    {
                        varientid = variant.varientid,
                        BatchNo = model.BatchNo,
                        QuantityIn = item.Quantity,
                        QuantityOut = 0,
                        CostPrice = item.Rate,
                        SellingPrice = item.SellingPrice,
                        CreatedDate = DateTime.Now
                    };

                    _context.InventoryBatch.Add(batch);

                    // update stock
                    variant.QuantityOnHand += item.Quantity;

                    // update selling price
                    var oldPrices = _context.VariantPrices
                        .Where(x => x.varientid == variant.varientid && x.IsActive)
                        .ToList();

                    foreach (var price in oldPrices)
                        price.IsActive = false;

                    _context.VariantPrices.Add(new VariantPrice
                    {
                        varientid = variant.varientid,
                        SellingPrice = item.SellingPrice,
                        IsActive = true
                    });
                }

                _context.SaveChanges();

                TempData["SuccessMessage"] = "Purchase Updated Successfully";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(model);
            }
        }


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
