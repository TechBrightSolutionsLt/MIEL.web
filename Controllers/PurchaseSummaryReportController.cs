using Microsoft.AspNetCore.Mvc;
using MIEL.web.Data;
using MIEL.web.Models.ViewModel;
using ClosedXML.Excel;
using System.IO;

namespace MIEL.web.Controllers
{
    public class PurchaseSummaryReportController : Controller
    {
        private readonly AppDBContext _context;
        public PurchaseSummaryReportController(AppDBContext context)
        {
            _context = context;

        }
        // ================== VIEW PURCHASE SUMMARY ==================
        public IActionResult PurchaseSummaryView(DateTime? FromDate, DateTime? ToDate, string InvoiceNo)
        {
            // Pass values to view
            ViewBag.FromDate = FromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = ToDate?.ToString("yyyy-MM-dd");
            ViewBag.InvoiceNo = InvoiceNo;

            // Return empty view if no search parameters
            if (!FromDate.HasValue && !ToDate.HasValue && string.IsNullOrEmpty(InvoiceNo))
                return View(null);

            // Base query: join PurchaseMaster with Supplier
            var query = from pm in _context.PurchaseMasters
                        join s in _context.Suppliers
                        on pm.SupplierId equals s.SupplierId
                        select new { pm, s };

            // Apply filters on entity properties
            if (FromDate.HasValue)
                query = query.Where(x => x.pm.InvoiceDate >= FromDate.Value);

            if (ToDate.HasValue)
                query = query.Where(x => x.pm.InvoiceDate <= ToDate.Value);

            if (!string.IsNullOrEmpty(InvoiceNo))
                query = query.Where(x => x.pm.InvoiceNo.Contains(InvoiceNo));

            // Project to ViewModel AFTER filtering
            var result = query.Select(x => new PurchaseSummaryVM
            {
                InvoiceNo = x.pm.InvoiceNo,
                SupplierName = x.s.Name,
                InvoiceDate = x.pm.InvoiceDate,
                GstAmount = x.pm.TotalTax,
                NetAmount = x.pm.TotalTaxable,
                TotalAmount = x.pm.TotalTax + x.pm.TotalTaxable
            }).ToList();

            return View(result);
        }

        // ================== EXPORT TO EXCEL ==================
        public IActionResult ExportToExcel(DateTime? FromDate, DateTime? ToDate, string InvoiceNo)
        {
            var query = from pm in _context.PurchaseMasters
                        join s in _context.Suppliers
                        on pm.SupplierId equals s.SupplierId
                        select new { pm, s };

            if (FromDate.HasValue)
                query = query.Where(x => x.pm.InvoiceDate >= FromDate.Value);

            if (ToDate.HasValue)
                query = query.Where(x => x.pm.InvoiceDate <= ToDate.Value);

            if (!string.IsNullOrEmpty(InvoiceNo))
                query = query.Where(x => x.pm.InvoiceNo.Contains(InvoiceNo));

            var data = query.Select(x => new PurchaseSummaryVM
            {
                InvoiceNo = x.pm.InvoiceNo,
                SupplierName = x.s.Name,
                InvoiceDate = x.pm.InvoiceDate,
                GstAmount = x.pm.TotalTax,
                NetAmount = x.pm.TotalTaxable,
                TotalAmount = x.pm.TotalTax + x.pm.TotalTaxable
            }).ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Purchase Summary");

                // Header
                worksheet.Cell(1, 1).Value = "Invoice No";
                worksheet.Cell(1, 2).Value = "Supplier Name";
                worksheet.Cell(1, 3).Value = "Purchase Date";
                worksheet.Cell(1, 4).Value = "Taxable Amount";
                worksheet.Cell(1, 5).Value = "GST";
                worksheet.Cell(1, 6).Value = "Total Amount";

                int row = 2;

                decimal totalNet = 0, totalGst = 0, totalAmount = 0;

                foreach (var item in data)
                {
                    worksheet.Cell(row, 1).Value = item.InvoiceNo;
                    worksheet.Cell(row, 2).Value = item.SupplierName;
                    worksheet.Cell(row, 3).Value = item.InvoiceDate.ToString("dd-MM-yyyy");
                    worksheet.Cell(row, 4).Value = item.NetAmount;
                    worksheet.Cell(row, 5).Value = item.GstAmount;
                    worksheet.Cell(row, 6).Value = item.TotalAmount;

                    totalNet += item.NetAmount;
                    totalGst += item.GstAmount;
                    totalAmount += item.TotalAmount;

                    row++;
                }

                // Footer totals
                worksheet.Cell(row, 3).Value = "TOTAL";
                worksheet.Cell(row, 4).Value = totalNet;
                worksheet.Cell(row, 5).Value = totalGst;
                worksheet.Cell(row, 6).Value = totalAmount;

                // Format header bold
                worksheet.Range(1, 1, 1, 6).Style.Font.Bold = true;
                worksheet.Range(row, 3, row, 6).Style.Font.Bold = true;

                var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;

                return File(stream.ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "PurchaseSummary.xlsx");
            }
        }
    }
}