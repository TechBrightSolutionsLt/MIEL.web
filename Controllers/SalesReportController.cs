using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MIEL.web.Data;
using MIEL.web.Models.ViewModel;
using System;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;

namespace MIEL.web.Controllers
{
    public class SalesReportController : Controller
    {
        private readonly AppDBContext _context;

        public SalesReportController(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(DateTime? fromDate, DateTime? toDate, int? customerId, int? salesMode, string paymentType)
        {
            var vm = new SalesReportVM
            {
                FromDate = fromDate ?? DateTime.Today,
                ToDate = toDate ?? DateTime.Today,
                CustomerId = customerId,
                SalesMode = salesMode,
                PaymentType = paymentType,
                Customers = await _context.users_TB.ToListAsync()
            };

            // If it's the initial load (original parameters were null), return empty list
            if (!fromDate.HasValue || !toDate.HasValue)
            {
                ViewBag.IsInitialLoad = true;
                return View(vm);
            }

            var query = from s in _context.SalesMasters
                        join u in _context.users_TB on s.CustomerId equals u.CustomerId
                        join si in _context.SalesItems on s.SalesId equals si.SalesId
                        join p in _context.ProColorSizeVariants on si.varientid equals p.varientid into pvGroup
                        from p in pvGroup.DefaultIfEmpty()
                        join n in _context.ProductMasters on p.ProductId equals n.ProductId into pmGroup
                        from n in pmGroup.DefaultIfEmpty()
                        select new { s, u, si, n };

            var from = fromDate.Value.Date;
            var to = toDate.Value.Date.AddDays(1);
            query = query.Where(x => x.s.SalesDate >= from && x.s.SalesDate < to);

            if (customerId.HasValue)
                query = query.Where(x => x.s.CustomerId == customerId);

            if (salesMode.HasValue)
                query = query.Where(x => x.s.salesmode == salesMode);

            if (!string.IsNullOrEmpty(paymentType))
                query = query.Where(x => x.s.PaymentType == paymentType);

            var data = await query
                .Select(x => new
                {
                    x.s.SalesId,
                    x.s.InvoiceNo,
                    x.s.SalesDate,
                    CustomerName = x.u.FirstName + " " + x.u.LastName,
                    ProductName = x.n != null ? x.n.ProductName : "",
                    BatchNumber = x.si.BatchNo ?? "",
                    Quantity = x.si.Quantity,
                    Rate = x.si.SellingPrice,
                    Discount = x.si.DiscAmount,
                    Tax = x.si.TaxAmount,
                    ItemTotal = x.si.NetAmount
                })
                .ToListAsync();

            var grouped = data
                .GroupBy(x => new
                {
                    x.SalesId,
                    x.InvoiceNo,
                    x.SalesDate,
                    x.CustomerName
                })
                .Select(g => {
                    var items = g.Select(i => new SalesReportItemVM
                    {
                        ProductName = i.ProductName,
                        BatchNumber = i.BatchNumber,
                        Quantity = (decimal)i.Quantity,
                        Rate = i.Rate,
                        NetAmt = (decimal)i.Quantity * i.Rate,
                        Discount = i.Discount,
                        Tax = i.Tax,
                        Taxable = ((decimal)i.Quantity * i.Rate) - i.Discount,
                        Total = i.ItemTotal
                    }).ToList();

                    return new SalesReportGroupVM
                    {
                        SalesId = g.Key.SalesId,
                        InvoiceNo = g.Key.InvoiceNo,
                        SalesDate = g.Key.SalesDate,
                        CustomerName = g.Key.CustomerName,
                        TotalNetAmt = items.Sum(x => x.NetAmt),
                        TotalDiscount = items.Sum(x => x.Discount),
                        TotalTax = items.Sum(x => x.Tax),
                        TotalTaxable = items.Sum(x => x.Taxable),
                        GrandTotal = items.Sum(x => x.Total),
                        Items = items
                    };
                })
                .OrderByDescending(x => x.SalesDate)
                .ToList();

            vm.Results = grouped;

            return View(vm);
        }
        //   public async Task<IActionResult> Export(DateTime? fromDate, DateTime? toDate,
        //int? customerId, int? salesMode, string paymentType)
        //   {
        //       if (!fromDate.HasValue && !toDate.HasValue)
        //       {
        //           fromDate = DateTime.Today;
        //           toDate = DateTime.Today;
        //       }
        //
        //       var query = from s in _context.SalesMasters
        //                   join u in _context.users_TB
        //                       on s.CustomerId equals u.CustomerId
        //                   join si in _context.SalesItems
        //                       on s.SalesId equals si.SalesId
        //                   join p in _context.ProColorSizeVariants
        //                       on si.varientid equals p.varientid into pvGroup
        //                   from p in pvGroup.DefaultIfEmpty()
        //                   join n in _context.ProductMasters
        //                       on p.ProductId equals n.ProductId into pmGroup
        //                   from n in pmGroup.DefaultIfEmpty()
        //                   select new { s, u, si, n };
        //
        //       if (fromDate.HasValue)
        //       {
        //           var from = fromDate.Value.Date;
        //           query = query.Where(x => x.s.SalesDate >= from);
        //       }
        //
        //       if (toDate.HasValue)
        //       {
        //           var to = toDate.Value.Date.AddDays(1);
        //           query = query.Where(x => x.s.SalesDate < to);
        //       }
        //
        //       if (customerId.HasValue)
        //           query = query.Where(x => x.s.CustomerId == customerId);
        //
        //       if (salesMode.HasValue)
        //           query = query.Where(x => x.s.salesmode == salesMode);
        //
        //       if (!string.IsNullOrEmpty(paymentType))
        //           query = query.Where(x => x.s.PaymentType == paymentType);
        //
        //       var data = await query
        //           .Select(x => new SalesReportResultVM
        //           {
        //               InvoiceNo = x.s.InvoiceNo,
        //               SalesDate = x.s.SalesDate,
        //               CustomerName = x.u.FirstName + " " + x.u.LastName,
        //               ProductName = x.n != null ? x.n.ProductName : "No Product",
        //               BatchNumber = x.si.BatchNo,
        //               NetAmount = x.s.NetAmount
        //           })
        //           .OrderByDescending(x => x.SalesDate)
        //           .ToListAsync();
        //
        //       using (var workbook = new XLWorkbook())
        //       {
        //           var worksheet = workbook.Worksheets.Add("Sales Report");
        //
        //           worksheet.Cell(1, 1).Value = "Invoice No";
        //           worksheet.Cell(1, 2).Value = "Date";
        //           worksheet.Cell(1, 3).Value = "Customer";
        //           worksheet.Cell(1, 4).Value = "Product";
        //           worksheet.Cell(1, 5).Value = "Batch";
        //           worksheet.Cell(1, 6).Value = "Net Amount";
        //
        //           int row = 2;
        //
        //           foreach (var item in data)
        //           {
        //               worksheet.Cell(row, 1).Value = item.InvoiceNo;
        //               worksheet.Cell(row, 2).Value = item.SalesDate.ToString("dd-MM-yyyy");
        //               worksheet.Cell(row, 3).Value = item.CustomerName;
        //               worksheet.Cell(row, 4).Value = item.ProductName;
        //               worksheet.Cell(row, 5).Value = item.BatchNumber;
        //               worksheet.Cell(row, 6).Value = item.NetAmount;
        //               row++;
        //           }
        //
        //           worksheet.Columns().AdjustToContents();
        //
        //           using (var stream = new MemoryStream())
        //           {
        //               workbook.SaveAs(stream);
        //               return File(stream.ToArray(),
        //                   "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        //                   "SalesReport.xlsx");
        //           }
        //       }
        //   }
        public async Task<IActionResult> Export(DateTime? fromDate, DateTime? toDate,
       int? customerId, int? salesMode, string paymentType)
        {
            if (!fromDate.HasValue && !toDate.HasValue)
            {
                fromDate = DateTime.Today;
                toDate = DateTime.Today;
            }

            var query = from s in _context.SalesMasters
                        join u in _context.users_TB
                            on s.CustomerId equals u.CustomerId
                        join si in _context.SalesItems
                            on s.SalesId equals si.SalesId
                        join p in _context.ProColorSizeVariants
                            on si.varientid equals p.varientid into pvGroup
                        from p in pvGroup.DefaultIfEmpty()
                        join n in _context.ProductMasters
                            on p.ProductId equals n.ProductId into pmGroup
                        from n in pmGroup.DefaultIfEmpty()
                        select new { s, u, si, n };

            if (fromDate.HasValue)
            {
                var from = fromDate.Value.Date;
                query = query.Where(x => x.s.SalesDate >= from);
            }

            if (toDate.HasValue)
            {
                var to = toDate.Value.Date.AddDays(1);
                query = query.Where(x => x.s.SalesDate < to);
            }

            if (customerId.HasValue)
                query = query.Where(x => x.s.CustomerId == customerId);

            if (salesMode.HasValue)
                query = query.Where(x => x.s.salesmode == salesMode);

            if (!string.IsNullOrEmpty(paymentType))
                query = query.Where(x => x.s.PaymentType == paymentType);

            var data = await query
                .Select(x => new
                {
                    x.s.InvoiceNo,
                    x.s.SalesDate,
                    CustomerName = x.u.FirstName + " " + x.u.LastName,
                    ProductName = x.n != null ? x.n.ProductName : "",
                    BatchNumber = x.si.BatchNo ?? "",
                    x.s.NetAmount
                })
                .OrderByDescending(x => x.SalesDate)
                .ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sales Detail Report");

                worksheet.Cell(1, 1).Value = "Invoice No";
                worksheet.Cell(1, 2).Value = "Date";
                worksheet.Cell(1, 3).Value = "Customer";
                worksheet.Cell(1, 4).Value = "Product";
                worksheet.Cell(1, 5).Value = "Batch";
                worksheet.Cell(1, 6).Value = "Net Amount";

                int row = 2;

                foreach (var item in data)
                {
                    worksheet.Cell(row, 1).Value = item.InvoiceNo ?? "";
                    worksheet.Cell(row, 2).Value = item.SalesDate;
                    worksheet.Cell(row, 2).Style.DateFormat.Format = "dd-MM-yyyy";
                    worksheet.Cell(row, 3).Value = item.CustomerName ?? "";
                    worksheet.Cell(row, 4).Value = item.ProductName ?? "";
                    worksheet.Cell(row, 5).Value = item.BatchNumber ?? "";
                    worksheet.Cell(row, 6).Value = item.NetAmount;

                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "SalesDetailReport.xlsx");
                }
            }
        }
    }
}
