using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MIEL.web.Data; // your DbContext namespace
using MIEL.web.Models.EntityModels;
using System.Threading.Tasks;
using System.Linq;
using System;

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
            var users = await _context.users_TB.ToListAsync();
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
                user.RoleId = 2;
                _context.Add(user);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "User created successfully!";
                return RedirectToAction(nameof(Index));
            }

            // If validation fails, stay on the same page
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
            if (id != user.CustomerId)
                return NotFound();

            // 🔥 Remove Password validation for Edit
            ModelState.Remove("Password");

            if (ModelState.IsValid)
            {
                var existingUser = await _context.users_TB.FindAsync(id);
                if (existingUser == null)
                    return NotFound();

                // Update only editable fields
                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
                existingUser.Email = user.Email;
                existingUser.MobileNumber = user.MobileNumber;
                existingUser.Gender = user.Gender;
                existingUser.Address = user.Address;
                existingUser.City = user.City;
                existingUser.Postcode = user.Postcode;

                // DO NOT TOUCH PASSWORD

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "User Updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }


        // ================= DELETE =================
        // No separate Delete view now; handled in Index via POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.users_TB.FindAsync(id);
            if (user != null)
            {
                _context.users_TB.Remove(user);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // ================= CHECK =================
        private bool UserExists(int id)
        {
            return _context.users_TB.Any(e => e.CustomerId == id);
        }
    }
}
