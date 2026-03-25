using Microsoft.AspNetCore.Mvc;
using MIEL.web.Data;
using MIEL.web.Models.ViewModel;

namespace MIEL.web.Controllers
{
    public class PurchaseSummaryReportController : Controller
    {
        private readonly AppDBContext _context;
        public PurchaseSummaryReportController(AppDBContext context)
        {
            _context = context;

        }
        public IActionResult PurchaseSummaryView(DateTime? FromDate, DateTime? ToDate, string InvoiceNo)
        {
            ViewBag.FromDate = FromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = ToDate?.ToString("yyyy-MM-dd");
            ViewBag.InvoiceNo = InvoiceNo;

            var query = from pm in _context.PurchaseMasters
                        select new PurchaseSummaryVM
                        {
                            InvoiceNo = pm.InvoiceNo,
                            InvoiceDate = pm.InvoiceDate,
                            GstAmount = pm.TotalTax,
                            NetAmount = pm.TotalTaxable,
                            TotalAmount = pm.TotalTax + pm.TotalTaxable
                        };

            if (FromDate.HasValue)
                query = query.Where(x => x.InvoiceDate >= FromDate.Value);

            if (ToDate.HasValue)
                query = query.Where(x => x.InvoiceDate <= ToDate.Value);

            if (!string.IsNullOrEmpty(InvoiceNo))
                query = query.Where(x => x.InvoiceNo.Contains(InvoiceNo));

            var result = query.ToList();

            return View(result);
        }
    }
}
