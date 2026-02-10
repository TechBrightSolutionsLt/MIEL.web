using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using MIEL.web.Data;
using MIEL.web.Models;
using System.Text;

namespace MIEL.web.Controllers
{
    public class PurchaseReportController : Controller
    {
        private readonly AppDBContext _context;

        public PurchaseReportController(AppDBContext context)
        {
            _context = context;
        }

        // ================== GET (Initial Load) ==================
        public IActionResult Index()
        {
            LoadSuppliers();

            ViewBag.FromDate = null;
            ViewBag.ToDate = null;
            ViewBag.SupplierId = null;

            var data = GetBaseQuery()
                        .Take(50)
                        .ToList();

            return View(data);
        }

        // ================== POST (Search) ==================
        [HttpPost]
        public IActionResult Index(DateTime? fromDate, DateTime? toDate, int? supplierId)
        {
            LoadSuppliers();

            // Preserve values for view + Excel
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");
            ViewBag.SupplierId = supplierId?.ToString();

            var query = GetBaseQuery();

            if (fromDate.HasValue)
                query = query.Where(x => x.InvoiceDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(x => x.InvoiceDate <= toDate.Value);

            if (supplierId.HasValue)
                query = query.Where(x => x.SupplierId == supplierId.Value);

            var result = query.ToList();

            if (result.Count == 0)
                ViewBag.Msg = "No records found for this search";

            return View(result);
        }

        // ================== EXPORT FILTERED DATA TO EXCEL ==================
        public IActionResult ExportToExcel(DateTime? fromDate, DateTime? toDate, int? supplierId)
        {
            var query = GetBaseQuery();

            if (fromDate.HasValue)
                query = query.Where(x => x.InvoiceDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(x => x.InvoiceDate <= toDate.Value);

            if (supplierId.HasValue)
                query = query.Where(x => x.SupplierId == supplierId.Value);

            var data = query.ToList();

            var sb = new StringBuilder();

            sb.AppendLine("InvoiceNo,InvoiceDate,Supplier,VariantCode,ProductName,Qty,Rate,DiscAmt,NetAmount,TotalTaxable,TotalTax");

            foreach (var r in data)
            {
                sb.AppendLine($"{r.InvoiceNo},{r.InvoiceDate:yyyy-MM-dd},{r.SupplierName},{r.VarientCode},{r.ProductName},{r.Quantity},{r.Rate},{r.DiscAmount},{r.NetAmount},{r.TotalTaxable},{r.TotalTax}");
            }

            byte[] buffer = Encoding.UTF8.GetBytes(sb.ToString());

            return File(buffer, "text/csv", "PurchaseReport.csv");
        }

        // ================== COMMON QUERY METHOD ==================
        private IQueryable<PurchaseReportVM> GetBaseQuery()
        {
            return from pi in _context.PurchaseItems
                   join pm in _context.PurchaseMasters
                       on pi.PurchaseId equals pm.PurchaseId
                   join s in _context.Suppliers
                       on pm.SupplierId equals s.SupplierId
                   join pcs in _context.ProColorSizeVariants
                       on pi.varientid equals pcs.varientid
                   join p in _context.ProductMasters
                       on pcs.ProductId equals p.ProductId
                   select new PurchaseReportVM
                   {
                       InvoiceNo = pm.InvoiceNo,
                       InvoiceDate = pm.InvoiceDate,
                       SupplierName = s.Name,
                       VarientCode = pcs.varientCode,
                       ProductName = p.ProductName,
                       Quantity = pi.Quantity,
                       Rate = pi.Rate,
                       DiscAmount = pi.DiscAmount,
                       NetAmount = pi.NetAmount,
                       TotalTaxable = pm.TotalTaxable,
                       TotalTax = pm.TotalTax,
                       SupplierId = s.SupplierId
                   };
        }

        // ================== LOAD SUPPLIERS (FIXED — NO ANONYMOUS TYPE) ==================
        private void LoadSuppliers()
        {
            ViewBag.Suppliers = _context.Suppliers
                .Select(s => new SelectListItem
                {
                    Value = s.SupplierId.ToString(),
                    Text = s.Name
                })
                .ToList();
        }
    }
}
