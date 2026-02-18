using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MIEL.web.Data;
using MIEL.web.Models.ViewModel;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MIEL.web.Controllers
{
    public class SalesReportController : Controller
    {
        private readonly AppDBContext _context;

        public SalesReportController(AppDBContext context)
        {
            _context = context;
        }

        //   public async Task<IActionResult> Index(
        //DateTime? fromDate,
        //DateTime? toDate,
        //int? customerId,
        //int? salesMode)
        //   {
        //       var query = from s in _context.SalesMasters
        //                   join c in _context.Customers
        //                   on s.CustomerId equals c.CustomerId
        //                   select new { s, c };

        //       if (fromDate.HasValue)
        //           query = query.Where(x => x.s.SalesDate >= fromDate.Value);

        //       if (toDate.HasValue)
        //           query = query.Where(x => x.s.SalesDate <= toDate.Value);

        //       if (customerId.HasValue)
        //           query = query.Where(x => x.s.CustomerId == customerId);

        //       if (salesMode.HasValue)
        //           query = query.Where(x => x.s.salesmode == salesMode);

        //       var data = await query
        //           .Select(x => new SalesReportResultVM
        //           {
        //               SalesId = x.s.SalesId,
        //               InvoiceNo = x.s.InvoiceNo,
        //               SalesDate = x.s.SalesDate,
        //               CustomerName = x.c.Name,
        //               SalesMode = x.s.salesmode,
        //               NetAmount = x.s.NetAmount
        //           })
        //           .OrderByDescending(x => x.SalesDate)
        //           .ToListAsync();

        //       var vm = new SalesReportVM
        //       {
        //           FromDate = fromDate,
        //           ToDate = toDate,
        //           CustomerId = customerId,
        //           SalesMode = salesMode,
        //           Results = data
        //       };

        //       vm.Customers = await _context.Customers.ToListAsync();

        //       return View(vm);
        //   }



        //    public async Task<IActionResult> Index(
        //DateTime? fromDate,
        //DateTime? toDate,
        //int? customerId,
        //int? salesMode,
        // string paymentType)
        //    {
        //        //var query = from s in _context.SalesMasters
        //        //            join u in _context.users_TB
        //        //            on s.CustomerId equals u.CustomerId
        //        //            select new { s, u };
        //        //var query = from s in _context.SalesMasters
        //        //            join u in _context.users_TB
        //        //                on s.CustomerId equals u.CustomerId
        //        //            join si in _context.SalesItems
        //        //                on s.SalesId equals si.SalesId
        //        //            join p in _context.ProColorSizeVariants
        //        //                on si.varientid equals p.varientid
        //        //                join n in _context.ProductMasters
        //        //               on  p.ProductId equals n.ProductId
        //        //            select new { s, u, si, p ,n};

        //        var query = from s in _context.SalesMasters
        //                    join si in _context.SalesItems
        //                        on s.SalesId equals si.SalesId

        //                    join p in _context.ProColorSizeVariants
        //                        on si.varientid equals p.varientid into pvGroup
        //                    from p in pvGroup.DefaultIfEmpty()   // 👈 LEFT JOIN

        //                    join n in _context.ProductMasters
        //                        on p.ProductId equals n.ProductId into pmGroup
        //                    from pm in pmGroup.DefaultIfEmpty()   // 👈 LEFT JOIN

        //                    select new SalesReportResultVM
        //                    {
        //                        SalesId = s.SalesId,
        //                        InvoiceNo = s.InvoiceNo,
        //                        SalesDate = s.SalesDate,
        //                        ProductName = pm != null ? pm.ProductName : "No Product",
        //                        BatchNumber = si.BatchNo,
        //                        NetAmount = s.NetAmount
        //                    };
        //        if (!fromDate.HasValue && !toDate.HasValue)
        //        {
        //            fromDate = DateTime.Today;
        //            toDate = DateTime.Today;
        //        }

        //        if (fromDate.HasValue)
        //        {
        //            var from = fromDate.Value.Date;
        //            query = query.Where(x => x.s.SalesDate >= from);
        //        }

        //        if (toDate.HasValue)
        //        {
        //            var to = toDate.Value.Date.AddDays(1);
        //            query = query.Where(x => x.s.SalesDate < to);
        //        }
        //        if (customerId.HasValue)
        //            query = query.Where(x => x.s.CustomerId == customerId);

        //        if (salesMode.HasValue)
        //            query = query.Where(x => x.s.salesmode == salesMode);
        //        if (!string.IsNullOrEmpty(paymentType))
        //            query = query.Where(x => x.s.PaymentType == paymentType);

        //        var data = await query
        //            .Select(x => new SalesReportResultVM
        //            {
        //                SalesId = x.s.SalesId,
        //                InvoiceNo = x.s.InvoiceNo,
        //                SalesDate = x.s.SalesDate,
        //                CustomerName = x.u.FirstName + " " + x.u.LastName,
        //               // SalesMode = x.s.salesmode,
        //               ProductName=x.n.ProductName,
        //                NetAmount = x.s.NetAmount
        //            })
        //            .OrderByDescending(x => x.SalesDate)
        //            .ToListAsync();

        //        var vm = new SalesReportVM
        //        {
        //            //FromDate = fromDate,
        //            //ToDate = toDate,
        //            FromDate = fromDate ?? DateTime.Today,
        //            ToDate = toDate ?? DateTime.Today,
        //            CustomerId = customerId,
        //            SalesMode = salesMode,
        //            PaymentType = paymentType,
        //            Results = data
        //        };
        //      //  vm.Customers = await _context.Customers.ToListAsync();
        //        // Load customers from users_TB
        //        vm.Customers = await _context.users_TB.ToListAsync();

        //        return View(vm);
        //    }
        public async Task<IActionResult> Index(
        DateTime? fromDate,
        DateTime? toDate,
        int? customerId,
        int? salesMode,
        string paymentType)
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
                .Select(x => new SalesReportResultVM
                {
                    SalesId = x.s.SalesId,
                    InvoiceNo = x.s.InvoiceNo,
                    SalesDate = x.s.SalesDate,
                    CustomerName = x.u.FirstName + " " + x.u.LastName,
                    ProductName = x.n != null ? x.n.ProductName : "No Product",
                    BatchNumber = x.si.BatchNo,
                    NetAmount = x.s.NetAmount
                })
                .OrderByDescending(x => x.SalesDate)
                .ToListAsync();

            var vm = new SalesReportVM
            {
                FromDate = fromDate ?? DateTime.Today,
                ToDate = toDate ?? DateTime.Today,
                CustomerId = customerId,
                SalesMode = salesMode,
                PaymentType = paymentType,
                Results = data,
                Customers = await _context.users_TB.ToListAsync()
            };

            return View(vm);
        }
    }
}