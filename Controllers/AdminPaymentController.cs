using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MIEL.web.Data;
using MIEL.web.Models.EntityModels;
using MIEL.web.Models.ViewModel;

public class AdminPaymentController : Controller
{
    private readonly AppDBContext _context;

    public AdminPaymentController(AppDBContext context)
    {
        _context = context;
    }

    // 1️⃣ Show Pending Orders with Search
    public async Task<IActionResult> PendingPayments(string search)
    {
        var query = _context.Orders
            .Include(x => x.Customer)
            .Where(x => x.PaymentStatus == "NotPaid")
            .AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            search = search.Trim().ToLower();

            query = query.Where(x =>
                x.OrderNumber.ToLower().Contains(search) ||
                x.Customer.FirstName.ToLower().Contains(search) ||
                x.Customer.LastName.ToLower().Contains(search) ||
                (x.Customer.FirstName + " " + x.Customer.LastName)
                    .ToLower().Contains(search)
            );
        }

        var orders = await query.ToListAsync();

        ViewBag.Search = search;

        return View(orders);
    }



    // 2️⃣ Show Verify Page
    [HttpGet]
    public async Task<IActionResult> Verify(int id)
    {
        var model = await _context.Orders
            .Where(o => o.Id == id)
            .Join(_context.SalesMasters,
                  o => o.SalesId,
                  s => s.SalesId,
                  (o, s) => new VerifyPaymentVM
                  {
                      Id = o.Id,
                      OrderNumber = o.OrderNumber,
                      TotalAmount = o.TotalAmount,
                      PaymentType = s.PaymentType,
                      BankReference = o.BankReference
                  })
            .FirstOrDefaultAsync();

        if (model == null)
            return NotFound();

        return View(model);
    }




    // 3️⃣ Confirm Payment
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Verify(VerifyPaymentVM model)
    {
        if (!ModelState.IsValid)
            return View("Verify", model);

        string customerId = HttpContext.Session.GetString("CustomerId");

        if (string.IsNullOrEmpty(customerId))
            return RedirectToAction("Login", "Account");

        int userstId = Convert.ToInt32(customerId);

        var order = await _context.Orders
            .FirstOrDefaultAsync(x => x.Id == model.Id);

        if (order == null)
            return NotFound();

        if (order.PaymentStatus == "Paid")
            return RedirectToAction("PendingPayments");

        order.PaymentStatus = "Paid";
        order.BankReference = model.BankReference;
        order.VerifyId = userstId;
        order.VerifiedDate = DateTime.Now;

        var sales = await _context.SalesMasters
            .FirstOrDefaultAsync(x => x.SalesId == order.SalesId);

        if (sales != null)
            sales.paysts = 1;

        await _context.SaveChangesAsync();

        return RedirectToAction("PendingPayments");
    }

}
