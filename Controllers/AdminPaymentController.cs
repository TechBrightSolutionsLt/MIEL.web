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

    // 1️⃣ Show Pending Orders
    public async Task<IActionResult> PendingPayments()
    {
        
        var orders = await _context.Orders
            .Where(x => x.PaymentStatus == "NotPaid")
            .Select(x => new OrderVM
            {
                Id = x.Id,
                CustomerId = x.CustomerId,
                OrderNumber = x.OrderNumber,
                TotalAmount = x.TotalAmount,
                PaymentStatus = x.PaymentStatus,
                PayId = x.PayId,
                VerifyId = x.VerifyId,
                BankReference = x.BankReference,
                VerifiedDate = x.VerifiedDate
            })
            .ToListAsync();

        return View(orders);
    }

    // 2️⃣ Show Verify Page
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
