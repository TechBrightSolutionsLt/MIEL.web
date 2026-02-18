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
                                  })
                                  .OrderBy(x => x.ProductName)   // ✅ ASCENDING
                                  .ThenBy(x => x.Colour)
                                  .ThenBy(x => x.Size)
                                  .ThenBy(x => x.BatchNo)
                                  .ToListAsync();

        var productSummary = batchDetails
            .GroupBy(x => x.ProductName)
            .Select(g => new ProductSummaryVM
            {
                ProductName = g.Key,
                TotalIn = g.Sum(x => x.QuantityIn),
                TotalOut = g.Sum(x => x.QuantityOut),
                CurrentStock = g.Sum(x => x.CurrentStock)
            })
            .OrderBy(x => x.ProductName)   // ✅ ASCENDING
            .ToList();

        var variantSummary = batchDetails
            .GroupBy(x => new { x.ProductName, x.Colour, x.Size })
            .Select(g => new VariantSummaryVM
            {
                ProductName = g.Key.ProductName,
                Colour = g.Key.Colour,
                Size = g.Key.Size,
                CurrentStock = g.Sum(x => x.CurrentStock)
            })
            .OrderBy(x => x.ProductName)   // ✅ ASCENDING
            .ThenBy(x => x.Colour)
            .ThenBy(x => x.Size)
            .ToList();

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
        var data = (from batch in _context.InventoryBatch
                    join variant in _context.ProColorSizeVariants
                        on batch.varientid equals variant.varientid
                    join product in _context.ProductMasters
                        on variant.ProductId equals product.ProductId
                    select new
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
                    })
                    .OrderBy(x => x.ProductName)
                    .ThenBy(x => x.Colour)
                    .ThenBy(x => x.Size)
                    .ToList();

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Stock Report");

        // Header Row
        ws.Cell(1, 1).Value = "Product";
        ws.Cell(1, 2).Value = "Colour";
        ws.Cell(1, 3).Value = "Size";
        ws.Cell(1, 4).Value = "Batch No";
        ws.Cell(1, 5).Value = "Qty In";
        ws.Cell(1, 6).Value = "Qty Out";
        ws.Cell(1, 7).Value = "Current";
        ws.Cell(1, 8).Value = "Cost Price";
        ws.Cell(1, 9).Value = "Selling Price";

        // Bold Header
        ws.Range(1, 1, 1, 9).Style.Font.Bold = true;

        int row = 2;

        foreach (var item in data)
        {
            ws.Cell(row, 1).Value = item.ProductName;
            ws.Cell(row, 2).Value = item.Colour;
            ws.Cell(row, 3).Value = item.Size;
            ws.Cell(row, 4).Value = item.BatchNo;
            ws.Cell(row, 5).Value = item.QuantityIn;
            ws.Cell(row, 6).Value = item.QuantityOut;
            ws.Cell(row, 7).Value = item.CurrentStock;
            ws.Cell(row, 8).Value = item.CostPrice;
            ws.Cell(row, 9).Value = item.SellingPrice;
            row++;
        }

        ws.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        return File(stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "StockReport.xlsx");
    }

}
