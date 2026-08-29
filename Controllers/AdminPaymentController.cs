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
        var query = from o in _context.Orders.Include(x => x.Customer)
                    join s in _context.SalesMasters on o.SalesId equals s.SalesId into sGroup
                    from s in sGroup.DefaultIfEmpty()
                    where o.PaymentStatus == "NotPaid"
                    select new { o, OrderDate = (DateTime?)s.SalesDate };

        if (!string.IsNullOrEmpty(search))
        {
            search = search.Trim().ToLower();

            query = query.Where(x =>
                x.o.OrderNumber.ToLower().Contains(search) ||
                x.o.Customer.FirstName.ToLower().Contains(search) ||
                x.o.Customer.LastName.ToLower().Contains(search) ||
                (x.o.Customer.FirstName + " " + x.o.Customer.LastName)
                    .ToLower().Contains(search)
            );
        }

        var data = await query.OrderByDescending(x => x.OrderDate).ToListAsync();

        var orders = data.Select(x => {
            x.o.OrderDate = x.OrderDate;
            return x.o;
        }).ToList();

        ViewBag.Search = search;

        return View(orders);
    }



    [HttpGet]
    public async Task<IActionResult> Verify(int id)
    {
        var model = await _context.Orders
            .Where(o => o.Id == id)

            // 🔹 Join SalesMaster
            .Join(_context.SalesMasters,
                  o => o.SalesId,
                  s => s.SalesId,
                  (o, s) => new { o, s })

            // 🔹 Join userModel (Customer)
            .Join(_context.users_TB,   // <-- your DbSet<userModel>
                  os => os.o.CustomerId,
                  u => u.CustomerId,
                  (os, u) => new VerifyPaymentVM
                  {
                      Id = os.o.Id,
                      OrderNumber = os.o.OrderNumber,
                      TotalAmount = os.o.TotalAmount,
                      PaymentType = os.s.PaymentType,
                      BankReference = os.o.BankReference,

                      // ✅ FROM userModel
                      Email = u.Email,
                      FirstName = u.FirstName
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
        TempData["SuccessMessage"] = "Payment confirmed successfully.";

        return RedirectToAction("PendingPayments");
    }

}
