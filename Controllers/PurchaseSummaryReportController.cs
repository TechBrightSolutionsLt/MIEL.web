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
        public IActionResult PurchaseSummaryView(DateTime? FromDate, DateTime? ToDate, string InvoiceNo)
        {
            ViewBag.FromDate = FromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = ToDate?.ToString("yyyy-MM-dd");
            ViewBag.InvoiceNo = InvoiceNo;


            // If no search parameters → return empty view
            if (!FromDate.HasValue && !ToDate.HasValue && string.IsNullOrEmpty(InvoiceNo))
            {
                return View(null);
            }

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

        public IActionResult ExportToExcel(DateTime? FromDate, DateTime? ToDate, string InvoiceNo)
        {
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
                query = query.Where(x => x.InvoiceDate < ToDate.Value.AddDays(1));

            if (!string.IsNullOrEmpty(InvoiceNo))
                query = query.Where(x => x.InvoiceNo.Contains(InvoiceNo));

            var data = query.ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Purchase Summary");

                worksheet.Cell(1, 1).Value = "Invoice No";
                worksheet.Cell(1, 2).Value = "Purchase Date";
                worksheet.Cell(1, 3).Value = "GST";
                worksheet.Cell(1, 4).Value = "Net Amount";
                worksheet.Cell(1, 5).Value = "Total Amount";

                int row = 2;

                foreach (var item in data)
                {
                    worksheet.Cell(row, 1).Value = item.InvoiceNo;
                    worksheet.Cell(row, 2).Value = item.InvoiceDate.ToString("dd-MM-yyyy");
                    worksheet.Cell(row, 3).Value = item.GstAmount;
                    worksheet.Cell(row, 4).Value = item.NetAmount;
                    worksheet.Cell(row, 5).Value = item.TotalAmount;
                    row++;
                }

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
