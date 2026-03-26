
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MIEL.web.Data;
using MIEL.web.Models.ViewModel;

namespace MIEL.web.Controllers
{
    public class SalessummaryController : Controller
    {
        private readonly AppDBContext _context;

        public SalessummaryController(AppDBContext context)
        {
            _context = context;
        }




        public async Task<IActionResult> Index(DateTime? fromDate, DateTime? toDate, string invoiceNo)
        {
            // Default = current month
            if (!fromDate.HasValue && !toDate.HasValue)
            {
                fromDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                toDate = DateTime.Today;
            }

            // Preserve UI values
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");
            ViewBag.InvoiceNo = invoiceNo;

            var query = _context.SalesMasters.AsQueryable();

            // Date Filters
            if (fromDate.HasValue)
            {
                var from = fromDate.Value.Date;
                query = query.Where(x => x.SalesDate >= from);
            }

            if (toDate.HasValue)
            {
                var to = toDate.Value.Date.AddDays(1);
                query = query.Where(x => x.SalesDate < to);
            }

            if (!string.IsNullOrWhiteSpace(invoiceNo))
            {
                var search = invoiceNo.Trim();

                query = query.Where(x =>
                    x.InvoiceNo != null &&
                    EF.Functions.Like(x.InvoiceNo, "%" + search + "%"));
            }

            var data = await query
                .Select(x => new SalesSummaryVM
                {
                    InvoiceNo = x.InvoiceNo,
                    SalesDate = x.SalesDate,
                    TaxableAmount = x.TotalAmount - x.TotalDiscount,
                    GSTAmount = x.GstAmount,
                    NetAmount = x.NetAmount
                })
                .OrderByDescending(x => x.SalesDate)
                .ToListAsync();

            return View(data);
        }



        [HttpGet]
        public async Task<IActionResult> GetInvoiceSuggestions(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Json(new List<string>());

            var invoices = await _context.SalesMasters
                .Where(x => x.InvoiceNo.Contains(term))
                .OrderByDescending(x => x.SalesDate)
                .Select(x => x.InvoiceNo)
                .Distinct()
                .Take(10)
                .ToListAsync();

            return Json(invoices);
        }
    }
}