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

                    
                    // 🔥 Reduce Batch Stock
                    batch.QuantityOut += item.Quantity;

                    // 🔥 Reduce Variant Stock (QuantityOnHand)
                    var variant = await _context.ProColorSizeVariants
                        .FirstOrDefaultAsync(v => v.varientid == item.varientid);

                    if (variant == null)
                        throw new Exception("Variant not found.");

                    if (variant.QuantityOnHand < item.Quantity)
                        throw new Exception("Variant stock mismatch.");

                    variant.QuantityOnHand -= item.Quantity;

                }

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                return Json(new
                {
                    success = true,
                    salesId = master.SalesId
                });


               
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();



                return Json(new
                {
                    success = false,
                    message = ex.Message
                });

            }
        }
        // ==========================================
        // PRINT A4 PAGE
        // ==========================================
        public async Task<IActionResult> Print(int id)
        {
            var sale = await _context.SalesMasters
                .Include(s => s.SalesItems)
                .FirstOrDefaultAsync(s => s.SalesId == id);

            if (sale == null)
                return NotFound();

            var vm = new SalesVM
            {
                SalesId = sale.SalesId,
                InvoiceNo = sale.InvoiceNo,
                SalesDate = sale.SalesDate,
                PaymentType = sale.PaymentType,
                TotalDiscount = sale.TotalDiscount,
                GstAmount = sale.GstAmount,
                NetAmount = sale.NetAmount,
                Items = sale.SalesItems.Select(i => new SalesItemVM
                {
                    varientid = i.varientid,
                    BatchNo = i.BatchNo,
                    Quantity = i.Quantity,
                    SellingPrice = i.SellingPrice,
                    DiscAmount = i.DiscAmount,
                    TaxAmount = i.TaxAmount,
                    NetAmount = i.NetAmount
                }).ToList()
            };

            return View(vm);
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
            var data = await _context.users_TB
                .Where(x => x.FirstName.Contains(term))
                .Select(x => new
                {
                    id = x.CustomerId,
                    text = x.FirstName
                })
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
                .Where(x => x.varientid == variantId && x.BatchNo == batchNo)
                .Select(x => new
                {
                    availableQty = x.QuantityOut,   // 🔥 YOUR REQUIREMENT
                    sellingPrice = x.SellingPrice
                })
                .FirstOrDefaultAsync();

            return Json(batch);
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
                .Include(s => s.SalesItems)   // ✅ correct navigation
                .FirstOrDefaultAsync(s => s.SalesId == id);

            if (sale == null)
                return NotFound();

            var vm = new SalesVM
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

                Items = sale.SalesItems.Select(d => new SalesItemVM
                {
                    varientid = d.varientid,
                    BatchNo = d.BatchNo,
                    Quantity = d.Quantity,
                    SellingPrice = d.SellingPrice,
                    DiscPercent = d.DiscPercent,
                    DiscAmount = d.DiscAmount,
                    TaxAmount = d.TaxAmount,
                    NetAmount = d.NetAmount
                }).ToList()
            };

            ViewBag.Customers = new SelectList(
                _context.users_TB.ToList(),
                "CustomerId",
                "FirstName",
                vm.CustomerId
            );

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SalesVM vm)
        {
            using var tx = await _context.Database.BeginTransactionAsync();

            try
            {
                var sale = await _context.SalesMasters
                    .Include(s => s.SalesItems)
                    .FirstOrDefaultAsync(s => s.SalesId == vm.SalesId);

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

                // 🔥 3. UPDATE HEADER
                sale.SalesDate = vm.SalesDate;
                sale.CustomerId = vm.CustomerId;
                sale.PaymentType = vm.PaymentType;
                sale.TotalAmount = vm.TotalAmount;
                sale.TotalDiscount = vm.TotalDiscount;
                sale.GstAmount = vm.GstAmount;
                sale.NetAmount = vm.NetAmount;

                // 🔥 4. ADD NEW ITEMS + REDUCE STOCK
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

                    sale.SalesItems.Add(new SalesItem
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
                    });

                    batch.QuantityOut += item.Quantity;
                }

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                return RedirectToAction("Details");
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                ModelState.AddModelError("", ex.Message);
                return View(vm);
            }
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
