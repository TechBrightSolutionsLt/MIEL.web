using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MIEL.web.Data; // your DbContext namespace
using MIEL.web.Models.EntityModels;
using System.Threading.Tasks;
using System.Linq;

namespace MIEL.web.Controllers
{
    public class CustomerUserController : Controller
    {
        private readonly AppDBContext _context;

        public CustomerUserController(AppDBContext context)
        {
            _context = context;
        }

        // ================= LIST =================
        public async Task<IActionResult> Index()
        {
            var users = await _context.users_TB.ToListAsync(); // Assuming DbSet<UserModel> Users
            return View(users);
        }

        // ================= CREATE =================
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(userModel user)
        {
            if (ModelState.IsValid)
            {
                user.CreatedDate = DateTime.Now;
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // ================= EDIT =================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.users_TB.FindAsync(id);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, userModel user)
        {
            if (id != user.CustomerId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.CustomerId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // ================= DELETE =================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.users_TB.FirstOrDefaultAsync(u => u.CustomerId == id);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.users_TB.FindAsync(id);
            _context.users_TB.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ================= CHECK =================
        private bool UserExists(int id)
        {
            return _context.users_TB.Any(e => e.CustomerId == id);
        }
    }
}
