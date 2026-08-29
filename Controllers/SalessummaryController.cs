using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MIEL.web.Data;
using MIEL.web.Models.ViewModel;
using ClosedXML.Excel;
using System.IO;

namespace MIEL.web.Controllers
{
    public class SalessummaryController : Controller
    {
        private readonly AppDBContext _context;

        public SalessummaryController(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(DateTime? fromDate, DateTime? toDate, string invoiceNo, string paymentType, string orderType)
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
            ViewBag.PaymentType = paymentType;
            ViewBag.OrderType = orderType;

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

            if (!string.IsNullOrWhiteSpace(paymentType))
            {
                query = query.Where(x => x.PaymentType == paymentType);
            }

            if (!string.IsNullOrWhiteSpace(orderType))
            {
                if (orderType == "Online")
                {
                    query = query.Where(x => x.salesmode == 2);
                }
                else if (orderType == "Direct")
                {
                    query = query.Where(x => x.salesmode != 2);
                }
            }

            var data = await query
                .Select(x => new SalesSummaryVM
                {
                    InvoiceNo = x.InvoiceNo,
                    SalesDate = x.SalesDate,
                    CustomerName = x.Customer != null
                        ? x.Customer.FirstName + " " + x.Customer.LastName
                        : "",
                    PaymentType = x.PaymentType,
                    OrderType = x.salesmode == 2 ? "Online" : "Direct",
                    TaxableAmount = x.TotalAmount -
                        (_context.SalesItems
                            .Where(i => i.SalesId == x.SalesId)
                            .Sum(i => (decimal?)i.TaxAmount) ?? 0),
                    GSTAmount = x.GstAmount,
                    NetAmount = x.NetAmount
                })
                .OrderBy(x => x.SalesDate)
                .ThenBy(x => x.InvoiceNo)
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

        public async Task<IActionResult> Export(DateTime? fromDate, DateTime? toDate, string invoiceNo, string paymentType, string orderType)
        {
            var query = _context.SalesMasters.AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(x => x.SalesDate >= fromDate.Value.Date);

            if (toDate.HasValue)
                query = query.Where(x => x.SalesDate < toDate.Value.Date.AddDays(1));

            if (!string.IsNullOrWhiteSpace(invoiceNo))
                query = query.Where(x => x.InvoiceNo.Contains(invoiceNo));

            if (!string.IsNullOrWhiteSpace(paymentType))
                query = query.Where(x => x.PaymentType == paymentType);

            if (!string.IsNullOrWhiteSpace(orderType))
            {
                if (orderType == "Online")
                    query = query.Where(x => x.salesmode == 2);
                else if (orderType == "Direct")
                    query = query.Where(x => x.salesmode != 2);
            }

            var data = await query
                .Select(x => new
                {
                    x.InvoiceNo,
                    x.SalesDate,
                    CustomerName = x.Customer != null ? x.Customer.FirstName + " " + x.Customer.LastName : "",
                    PaymentType = x.PaymentType ?? "",
                    OrderType = x.salesmode == 2 ? "Online" : "Direct",
                    TaxableAmount = x.TotalAmount -
                        (_context.SalesItems
                            .Where(i => i.SalesId == x.SalesId)
                            .Sum(i => (decimal?)i.TaxAmount) ?? 0),
                    x.GstAmount,
                    x.NetAmount
                })
                .OrderBy(x => x.SalesDate)
                .ThenBy(x => x.InvoiceNo)
                .ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("Sales Summary");

                // Header
                ws.Cell(1, 1).Value = "Invoice No";
                ws.Cell(1, 2).Value = "Date";
                ws.Cell(1, 3).Value = "Customer";
                ws.Cell(1, 4).Value = "Payment Type";
                ws.Cell(1, 5).Value = "Order Type";
                ws.Cell(1, 6).Value = "Taxable";
                ws.Cell(1, 7).Value = "GST";
                ws.Cell(1, 8).Value = "Net Amount";

                int row = 2;

                decimal totalTaxable = 0;
                decimal totalGST = 0;
                decimal totalNet = 0;

                foreach (var item in data)
                {
                    ws.Cell(row, 1).Value = item.InvoiceNo;
                    ws.Cell(row, 2).Value = item.SalesDate;
                    ws.Cell(row, 2).Style.DateFormat.Format = "dd-MM-yyyy";
                    ws.Cell(row, 3).Value = item.CustomerName;
                    ws.Cell(row, 4).Value = item.PaymentType;
                    ws.Cell(row, 5).Value = item.OrderType;
                    ws.Cell(row, 6).Value = item.TaxableAmount;
                    ws.Cell(row, 7).Value = item.GstAmount;
                    ws.Cell(row, 8).Value = item.NetAmount;

                    totalTaxable += item.TaxableAmount;
                    totalGST += item.GstAmount;
                    totalNet += item.NetAmount;

                    row++;
                }

                // Totals row
                ws.Cell(row, 5).Value = "TOTAL";
                ws.Cell(row, 6).Value = totalTaxable;
                ws.Cell(row, 7).Value = totalGST;
                ws.Cell(row, 8).Value = totalNet;

                ws.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "SalesSummary.xlsx");
                }
            }
        }

        public async Task<IActionResult> Print(DateTime? fromDate, DateTime? toDate, string invoiceNo, string paymentType, string orderType)
        {
            // Same default date logic as Index
            if (!fromDate.HasValue && !toDate.HasValue)
            {
                fromDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                toDate = DateTime.Today;
            }

            var query = _context.SalesMasters.AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(x => x.SalesDate >= fromDate.Value.Date);

            if (toDate.HasValue)
                query = query.Where(x => x.SalesDate < toDate.Value.Date.AddDays(1));

            if (!string.IsNullOrWhiteSpace(invoiceNo))
                query = query.Where(x => x.InvoiceNo.Contains(invoiceNo));

            if (!string.IsNullOrWhiteSpace(paymentType))
                query = query.Where(x => x.PaymentType == paymentType);

            if (!string.IsNullOrWhiteSpace(orderType))
            {
                if (orderType == "Online")
                    query = query.Where(x => x.salesmode == 2);
                else if (orderType == "Direct")
                    query = query.Where(x => x.salesmode != 2);
            }

            var data = await query
                .Select(x => new SalesSummaryVM
                {
                    InvoiceNo = x.InvoiceNo,
                    SalesDate = x.SalesDate,
                    CustomerName = x.Customer != null ? x.Customer.FirstName + " " + x.Customer.LastName : "",
                    PaymentType = x.PaymentType,
                    OrderType = x.salesmode == 2 ? "Online" : "Direct",
                    TaxableAmount = x.TotalAmount -
                        (_context.SalesItems
                            .Where(i => i.SalesId == x.SalesId)
                            .Sum(i => (decimal?)i.TaxAmount) ?? 0),
                    GSTAmount = x.GstAmount,
                    NetAmount = x.NetAmount
                })
                .OrderBy(x => x.SalesDate)
                .ThenBy(x => x.InvoiceNo)
                .ToListAsync();

            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");
            ViewBag.InvoiceNo = invoiceNo;
            ViewBag.PaymentType = paymentType;
            ViewBag.OrderType = orderType;

            return View(data);
        }
    }
}
