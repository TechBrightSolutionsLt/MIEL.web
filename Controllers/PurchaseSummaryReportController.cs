using Microsoft.AspNetCore.Mvc;
using MIEL.web.Data;

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
            // Later search logic add here

            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            ViewBag.InvoiceNo = InvoiceNo;

            return View();
        }
    }
}
