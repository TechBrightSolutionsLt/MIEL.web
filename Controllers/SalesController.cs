using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MIEL.web.Data;
using MIEL.web.Models.EntityModels;
using MIEL.web.Models.ViewModel;
using System;
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
                var master = new SalesMaster
                {
                    InvoiceNo = vm.InvoiceNo,
                    SalesDate = vm.SalesDate,
                    PaymentType = vm.PaymentType,
                    TotalAmount = vm.TotalAmount,
                    TotalDiscount = vm.TotalDiscount,
                    GstAmount = vm.GstAmount,
                    NetAmount = vm.NetAmount
                };

                _context.SalesMasters.Add(master);
                await _context.SaveChangesAsync();

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




    }
}
