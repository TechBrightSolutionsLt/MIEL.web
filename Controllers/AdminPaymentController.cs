using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MIEL.web.Data;
using MIEL.web.Models.EntityModels;

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
        var orders = await _context.Set<OrderVM>()
            .Where(x => x.PaymentStatus == "NotPaid")
            .ToListAsync();

        return View(orders);
    }

    // 2️⃣ Show Verify Page
    public async Task<IActionResult> Verify(int id)
    {
        var order = await _context.Set<OrderVM>()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (order == null)
            return NotFound();

        return View(order);
    }

    // 3️⃣ Confirm Payment
    [HttpPost]
    public async Task<IActionResult> Verify(OrderVM model)
    {
        var order = await _context.Set<OrderVM>()
            .FirstOrDefaultAsync(x => x.Id == model.Id);

        if (order == null)
            return NotFound();

        order.PaymentStatus = "Paid";
        order.BankReference = model.BankReference;
        order.VerifyId = 1; // Replace with logged-in Admin Id
        order.VerifiedDate = DateTime.Now;

        await _context.SaveChangesAsync();

        return RedirectToAction("PendingPayments");
    }
}
