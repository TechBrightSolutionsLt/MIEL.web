using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MIEL.web.Data;
using MIEL.web.Models.ViewModel;

public class StockController : Controller
{
    private readonly AppDBContext _context;

    public StockController(AppDBContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(DateTime? fromDate, DateTime? toDate)
    {
        var batchQuery = _context.InventoryBatch.AsQueryable();

        if (fromDate.HasValue)
            batchQuery = batchQuery.Where(x => x.CreatedDate >= fromDate);

        if (toDate.HasValue)
            batchQuery = batchQuery.Where(x => x.CreatedDate <= toDate);

        var batchDetails = await (from batch in batchQuery
                                  join variant in _context.ProColorSizeVariants
                                      on batch.varientid equals variant.varientid
                                  join product in _context.ProductMasters
                                      on variant.ProductId equals product.ProductId
                                  select new BatchDetailVM
                                  {
                                      ProductName = product.ProductName,
                                      Colour = variant.colour,
                                      Size = variant.size,
                                      BatchNo = batch.BatchNo,
                                      QuantityIn = batch.QuantityIn,
                                      QuantityOut = batch.QuantityOut,
                                      CurrentStock = batch.QuantityIn - batch.QuantityOut,
                                      CostPrice = batch.CostPrice,
                                      SellingPrice = batch.SellingPrice
                                  }).ToListAsync();

        var productSummary = batchDetails
            .GroupBy(x => x.ProductName)
            .Select(g => new ProductSummaryVM
            {
                ProductName = g.Key,
                TotalIn = g.Sum(x => x.QuantityIn),
                TotalOut = g.Sum(x => x.QuantityOut),
                CurrentStock = g.Sum(x => x.CurrentStock)
            }).ToList();

        var variantSummary = batchDetails
            .GroupBy(x => new { x.ProductName, x.Colour, x.Size })
            .Select(g => new VariantSummaryVM
            {
                ProductName = g.Key.ProductName,
                Colour = g.Key.Colour,
                Size = g.Key.Size,
                CurrentStock = g.Sum(x => x.CurrentStock)
            }).ToList();

        var vm = new StockReportVM
        {
            FromDate = fromDate,
            ToDate = toDate,
            BatchDetails = batchDetails,
            ProductSummary = productSummary,
            VariantSummary = variantSummary
        };

        return View(vm);
    }

    // EXPORT EXCEL
    public IActionResult ExportExcel()
    {
        var data = _context.InventoryBatch.ToList();

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Stock Report");

        ws.Cell(1, 1).Value = "Batch No";
        ws.Cell(1, 2).Value = "Qty In";
        ws.Cell(1, 3).Value = "Qty Out";

        int row = 2;
        foreach (var item in data)
        {
            ws.Cell(row, 1).Value = item.BatchNo;
            ws.Cell(row, 2).Value = item.QuantityIn;
            ws.Cell(row, 3).Value = item.QuantityOut;
            row++;
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        return File(stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "StockReport.xlsx");
    }
}
