using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MIEL.web.Data;
using MIEL.web.Models.EntityModels;
using MIEL.web.Models.ViewModel;
using System;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading.Tasks;



namespace MIEL.web.Controllers
{
    public class SalesController : Controller
    {
        private readonly AppDBContext _context;

        public SalesController(AppDBContext context)
        {
            _context = context;
        }
        // =====================================================
        // CREATE GET
        // =====================================================
        public IActionResult Create()
        {
            var vm = new SalesVM
            {
                InvoiceNo = "SAL-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                SalesDate = DateTime.Today
            };

            ViewBag.Customers = new SelectList(
                _context.users_TB.ToList(),
                "CustomerId",
                "FirstName"
            );

            return View(vm);
        }


        // =====================================================
        // CREATE POST
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SalesVM vm)
        {
            if (vm.Items == null || vm.Items.Count == 0)
            {
                ModelState.AddModelError("", "Please add items.");

                ViewBag.Customers = new SelectList(
                    _context.users_TB.ToList(),
                    "CustomerId",
                    "FirstName",
                    vm.CustomerId
                );

                return View(vm);
            }

            using var tx = await _context.Database.BeginTransactionAsync();

            try
            {
                // =============================
                // 🟢 INSERT MODE
                // =============================
                var master = new SalesMaster
                {
                    InvoiceNo = vm.InvoiceNo,
                    SalesDate = vm.SalesDate,
                    CustomerId = vm.CustomerId,
                    PaymentType = vm.PaymentType,
                    TotalAmount = vm.TotalAmount,
                    TotalDiscount = vm.TotalDiscount,
                    GstAmount = vm.GstAmount,
                    NetAmount = vm.NetAmount,
                    paysts = 1,      // 🔥 Always 1
                    salesmode = 1    // 🔥 Always 1
                };

                _context.SalesMasters.Add(master);
                await _context.SaveChangesAsync();

                // =============================
                // ADD ITEMS
                // =============================
                foreach (var item in vm.Items)
                {
                    var batch = await _context.InventoryBatch
                        .FirstOrDefaultAsync(x =>
                            x.varientid == item.varientid &&
                            x.BatchNo == item.BatchNo);

                    if (batch == null)
                        throw new Exception("Batch not found.");

                    int available = batch.QuantityIn - batch.QuantityOut;

                    if (available < item.Quantity)
                        throw new Exception("Insufficient stock.");

                    var salesItem = new SalesItem
                    {
                        SalesId = master.SalesId,
                        varientid = item.varientid,
                        BatchNo = item.BatchNo,
                        Quantity = item.Quantity,
                        SellingPrice = item.SellingPrice,
                        DiscPercent = item.DiscPercent,
                        DiscAmount = item.DiscAmount,
                        TaxAmount = item.TaxAmount,
                        NetAmount = item.NetAmount
                    };

                    _context.SalesItems.Add(salesItem);

                    // 🔥 Reduce stock
                    batch.QuantityOut += item.Quantity;
                }

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                TempData["SuccessMessage"] = "Sales saved successfully!";

                return RedirectToAction("Create");
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();

                ViewBag.Customers = new SelectList(
                    _context.users_TB.ToList(),
                    "CustomerId",
                    "FirstName",
                    vm.CustomerId
                );

                ModelState.AddModelError("", ex.Message);
                return View(vm);
            }
        }

        // =====================================================
        // SEARCH PRODUCT
        // =====================================================
        public async Task<IActionResult> SearchProducts(string term)
        {
            var data = await _context.ProductMasters
                .Where(x => x.ProductName.Contains(term))
                .Select(x => new { id = x.ProductId, text = x.ProductName })
                .Take(20)
                .ToListAsync();

            return Json(data);
        }

        // SEARCH CUSTOMER
        public async Task<IActionResult> SearchCustomers(string term)
        {
            var data = await _context.Customers
                .Where(x => x.Name.Contains(term))
                .Select(x => new { id = x.CustomerId, text = x.Name })
                .Take(20)
                .ToListAsync();

            return Json(data);
        }

        // LOAD VARIANTS
        public async Task<IActionResult> GetVariants(int productId)
        {
            var data = await _context.ProColorSizeVariants
                .Where(x => x.ProductId == productId)
                .Select(x => new
                {
                    id = x.varientid,
                    text = x.colour + " - " + x.size
                })
                .ToListAsync();

            return Json(data);
        }

        public async Task<IActionResult> GetBatches(int variantId)
        {
            var data = await _context.InventoryBatch
                .Where(x => x.varientid == variantId &&
                            (x.QuantityIn - x.QuantityOut) > 0)
                .Select(x => new
                {
                    batchNo = x.BatchNo,
                    availableQty = x.QuantityIn - x.QuantityOut
                })
                .ToListAsync();

            return Json(data);
        }

        public async Task<IActionResult> GetBatchDetails(int variantId, string batchNo)
        {
            var batch = await _context.InventoryBatch
                .FirstOrDefaultAsync(x =>
                    x.varientid == variantId &&
                    x.BatchNo == batchNo);

            if (batch == null)
            {
                return Json(new
                {
                    availableQty = 0,
                    sellingPrice = 0
                });
            }

            return Json(new
            {
                availableQty = batch.QuantityIn - batch.QuantityOut,
                sellingPrice = batch.CostPrice   // ✅ CHANGE HERE
            });
        }

        public async Task<IActionResult> Details()
        {
            var result = await _context.SalesMasters
        .Select(s => new SalesVM
        {
            SalesId = s.SalesId,   // 🔥 VERY IMPORTANT

            //var result = await _context.SalesMasters
            //    .Select(s => new SalesVM
            //    {
            //         SalesId = s.SalesId,   // 👈 ADD THIS (VERY IMPORTANT)
                    InvoiceNo = s.InvoiceNo,
                    SalesDate = s.SalesDate,
                    PaymentType = s.PaymentType,
                    NetAmount = s.NetAmount,
                    Items = _context.SalesItems
                                .Where(i => i.SalesId == s.SalesId)
                                .Select(i => new SalesItemVM
                                {
                                    varientid = i.varientid,
                                    BatchNo = i.BatchNo,
                                    Quantity = i.Quantity,
                                    SellingPrice = i.SellingPrice,
                                    DiscAmount = i.DiscAmount,
                                    TaxAmount = i.TaxAmount,
                                    NetAmount = i.NetAmount
                                }).ToList()
                }).ToListAsync();

            return View(result);
        }

        // ==========================================
        // EDIT GET
        // ==========================================
        public async Task<IActionResult> Edit(int id)
        {
            var sale = await _context.SalesMasters
                .Include(s => s.SalesItems)
                .FirstOrDefaultAsync(s => s.SalesId == id);

            if (sale == null)
                return NotFound();

            ViewBag.Customers = new SelectList(
                _context.users_TB.ToList(),
                "CustomerId",
                "FirstName",
                sale.CustomerId   // 🔥 THIS selects current customer
            );

            var model = new SalesVM
            {
                SalesId = sale.SalesId,
                InvoiceNo = sale.InvoiceNo,
                SalesDate = sale.SalesDate,
                CustomerId = sale.CustomerId,
                PaymentType = sale.PaymentType,
                TotalAmount = sale.TotalAmount,
                TotalDiscount = sale.TotalDiscount,
                GstAmount = sale.GstAmount,
                NetAmount = sale.NetAmount,
                Items = sale.SalesItems.Select(i => new SalesItemVM
                {
                    varientid = i.varientid,
                    BatchNo = i.BatchNo,
                    Quantity = i.Quantity,
                    SellingPrice = i.SellingPrice,
                    DiscPercent = i.DiscPercent,
                    DiscAmount = i.DiscAmount,
                    TaxAmount = i.TaxAmount,
                    NetAmount = i.NetAmount
                }).ToList()
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SalesVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var sale = await _context.SalesMasters
                .Include(s => s.SalesItems)
                .FirstOrDefaultAsync(s => s.SalesId == model.SalesId);

            if (sale == null)
                return NotFound();

            // 🔥 1. RESTORE OLD STOCK
            foreach (var oldItem in sale.SalesItems)
            {
                var batch = await _context.InventoryBatch
                    .FirstOrDefaultAsync(x =>
                        x.varientid == oldItem.varientid &&
                        x.BatchNo == oldItem.BatchNo);

                if (batch != null)
                    batch.QuantityOut -= oldItem.Quantity;
            }

            // 🔥 2. REMOVE OLD ITEMS
            _context.SalesItems.RemoveRange(sale.SalesItems);

            // 🔥 3. UPDATE MASTER
            sale.SalesDate = model.SalesDate;
            sale.CustomerId = model.CustomerId;
            sale.TotalAmount = model.TotalAmount;
            sale.TotalDiscount = model.TotalDiscount;
            sale.GstAmount = model.GstAmount;
            sale.NetAmount = model.NetAmount;

            // 🔥 4. ADD NEW ITEMS
            foreach (var item in model.Items)
            {
                var newItem = new SalesItem
                {
                    SalesId = sale.SalesId,
                    varientid = item.varientid,
                    BatchNo = item.BatchNo,
                    Quantity = item.Quantity,
                    SellingPrice = item.SellingPrice,
                    DiscPercent = item.DiscPercent,
                    DiscAmount = item.DiscAmount,
                    TaxAmount = item.TaxAmount,
                    NetAmount = item.NetAmount
                };

                _context.SalesItems.Add(newItem);

                var batch = await _context.InventoryBatch
                    .FirstOrDefaultAsync(x =>
                        x.varientid == item.varientid &&
                        x.BatchNo == item.BatchNo);

                if (batch != null)
                    batch.QuantityOut += item.Quantity;
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Sale Updated Successfully!";
            return RedirectToAction("Details");
        }

        // ==========================================
        // DELETE
        // ==========================================
        public async Task<IActionResult> Delete(int id)
        {
            var sale = await _context.SalesMasters
                .Include(x => x.SalesItems)
                .FirstOrDefaultAsync(x => x.SalesId == id);

            if (sale == null)
                return NotFound();

            // Remove child items first
            _context.SalesItems.RemoveRange(sale.SalesItems);

            // Remove master
            _context.SalesMasters.Remove(sale);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Sale deleted successfully!";
            return RedirectToAction("Details");
        }



    }
}
