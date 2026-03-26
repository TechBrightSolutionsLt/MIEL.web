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

            var today = DateTime.Today;
            ViewBag.FromDate = today.ToString("yyyy-MM-dd");
            ViewBag.ToDate = today.ToString("yyyy-MM-dd");
            ViewBag.SupplierId = null;
            ViewBag.IsInitialLoad = false;

            var result = FilterQuery(today, today, null, null, null).ToList();

            return View(result);
        }

        // ================== POST (Search) ==================
        [HttpPost]
        public IActionResult Index(DateTime? fromDate, DateTime? toDate, int? supplierId, string invoiceNo, string productName)
        {
            LoadSuppliers();

            // Preserve values for view + Excel, defaulting to today if null
            var todayStr = DateTime.Today.ToString("yyyy-MM-dd");
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd") ?? todayStr;
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd") ?? todayStr;
            ViewBag.SupplierId = supplierId?.ToString();
            ViewBag.InvoiceNo = invoiceNo;
            ViewBag.ProductName = productName;

            var result = FilterQuery(fromDate ?? DateTime.Today, toDate ?? DateTime.Today, supplierId, invoiceNo, productName).ToList();

            if (result.Count == 0)
                ViewBag.Msg = "No records found for this search";

            return View(result);
        }

        // ================== EXPORT FILTERED DATA TO EXCEL ==================
        public IActionResult ExportToExcel(DateTime? fromDate, DateTime? toDate, int? supplierId, string invoiceNo, string productName)
        {
            var data = FilterQuery(fromDate, toDate, supplierId, invoiceNo, productName).ToList();

            var sb = new StringBuilder();

            sb.AppendLine("InvoiceNo,InvoiceDate,Supplier,VariantCode,ProductName,Qty,Rate,Taxable,Discount,Tax");

            foreach (var r in data)
            {
                var gross = r.Quantity * r.Rate;
                sb.AppendLine($"{r.InvoiceNo},{r.InvoiceDate:yyyy-MM-dd},{r.SupplierName},{r.VarientCode},{r.ProductName},{r.Quantity},{r.Rate},{gross},{r.DiscAmount},{r.GstAmount}");
            }

            byte[] buffer = Encoding.UTF8.GetBytes(sb.ToString());

            return File(buffer, "text/csv", "PurchaseReport.csv");
        }

        // ================== HELPER FILTER METHOD ==================
        private IQueryable<PurchaseReportVM> FilterQuery(DateTime? fromDate, DateTime? toDate, int? supplierId, string invoiceNo, string productName)
        {
            var query = GetBaseQuery();

            if (fromDate.HasValue)
                query = query.Where(x => x.InvoiceDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(x => x.InvoiceDate <= toDate.Value);

            if (supplierId.HasValue)
                query = query.Where(x => x.SupplierId == supplierId.Value);

            if (!string.IsNullOrEmpty(invoiceNo))
                query = query.Where(x => x.InvoiceNo.Contains(invoiceNo));

            if (!string.IsNullOrEmpty(productName))
                query = query.Where(x => x.ProductName.Contains(productName));

            return query;
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
                       BatchNo = pi.BatchNo,
                       GstAmount = pi.GstAmount,
                       UnitName = pcs.size,
                       SupInvNo = "", 
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
