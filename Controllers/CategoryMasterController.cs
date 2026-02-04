using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MIEL.web.Data;
using MIEL.web.Models.EntityModels;
using MIEL.web.Models.ViewModels;

namespace MIEL.web.Controllers
{
    public class CategoryMasterController : Controller
    {
        private readonly AppDBContext _context;

        public CategoryMasterController(AppDBContext context)
        {
            _context = context;
        }

        // GET : Category Master
        public IActionResult CategoryMaster(int? id)
        {
            var vm = new CategoryMasterVM
            {
                Categories = _context.Categories.ToList(),
                Category = id == null
                    ? new Category()
                    : _context.Categories.FirstOrDefault(c => c.CategoryId == id)
            };

            return View("~/Views/Admin/CategoryMaster.cshtml", vm);
        }

        // POST : Upsert
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CategoryMasterVM vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Categories = _context.Categories.ToList();
                return View("~/Views/Admin/CategoryMaster.cshtml", vm);
            }

            if (vm.Category.CategoryId == 0)
            {
                // INSERT
                _context.Categories.Add(vm.Category);
            }
            else
            {
                // UPDATE
                var existing = _context.Categories
                    .FirstOrDefault(c => c.CategoryId == vm.Category.CategoryId);

                if (existing == null)
                    return NotFound();

                existing.CategoryName = vm.Category.CategoryName;
            }

            _context.SaveChanges();

            return RedirectToAction(nameof(CategoryMaster));
        }

        // POST : Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var category = _context.Categories
                .FirstOrDefault(c => c.CategoryId == id);

            if (category == null)
                return NotFound();

            _context.Categories.Remove(category);
            _context.SaveChanges();

            return RedirectToAction(nameof(CategoryMaster));
        }
    }
}
