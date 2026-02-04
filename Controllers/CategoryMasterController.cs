using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MIEL.web.Data;
using YourNamespace.Models;

namespace YourNamespace.Controllers
{
    public class CategoryMasterController : Controller
    {
        private readonly AppDBContext _context;

        public CategoryMasterController(AppDBContext context)
        {
            _context = context;
        }

        // GET: CategoryMaster
        public IActionResult CategoryMaster()
        {
            var categories = _context.Categories.ToList();
            return View("~/Views/Admin/CategoryMaster.cshtml",categories);
        }

        // GET: Create or Edit
        public IActionResult Upsert(int? id)
        {
            if (id == null)
                return View(new Category()); // Create

            var category = _context.Categories.FirstOrDefault(c => c.CategoryId == id);
            if (category == null)
                return NotFound();

            return View(category); // Edit
        }

        // POST: Create or Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Category category)
        {
            if (!ModelState.IsValid)
                return View(category);

            if (category.CategoryId == 0)
            {
                _context.Categories.Add(category); // Create
            }
            else
            {
                _context.Categories.Update(category); // Edit
            }

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // GET: Delete
        public IActionResult Delete(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.CategoryId == id);
            if (category == null)
                return NotFound();

            _context.Categories.Remove(category);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
