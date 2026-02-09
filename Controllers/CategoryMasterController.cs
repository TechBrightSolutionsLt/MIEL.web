using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MIEL.web.Data;
using MIEL.web.Models.EntityModels;
using MIEL.web.Models.ViewModels;
using System.Linq;

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
        //public IActionResult CategoryMaster(int? id)
        //{
        //    var vm = new CategoryMasterVM
        //    {
        //        Categories = _context.Categories.ToList(),
        //        Category = id == null
        //            ? new Category()
        //            : _context.Categories.FirstOrDefault(c => c.CategoryId == id)
        //    };

        //    return View("~/Views/Admin/CategoryMaster.cshtml", vm);
        //}
        public IActionResult CategoryMaster(int? id)
        {
            var vm = new CategoryMasterVM
            {
                Categories = _context.Categories
                    .Include(c => c.MainCategory) // ⭐ REQUIRED
                    .ToList(),

                Category = id == null
                    ? new Category()
                    : _context.Categories.FirstOrDefault(c => c.CategoryId == id),

                MainCategoryList = _context.MainCategories
                    .Select(m => new SelectListItem
                    {
                        Text = m.MainCategoryName,
                        Value = m.MainCategoryId.ToString()
                    })
                    .ToList()
            };

            return View("~/Views/Admin/CategoryMaster.cshtml", vm);
        }
        // POST : Upsert
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Upsert(CategoryMasterVM vm)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        vm.Categories = _context.Categories.ToList();
        //        return View("~/Views/Admin/CategoryMaster.cshtml", vm);
        //    }

        //    if (vm.Category.CategoryId == 0)
        //    {
        //        // INSERT
        //        _context.Categories.Add(vm.Category);
        //    }
        //    else
        //    {
        //        // UPDATE
        //        var existing = _context.Categories
        //            .FirstOrDefault(c => c.CategoryId == vm.Category.CategoryId);

        //        if (existing == null)
        //            return NotFound();

        //        existing.CategoryName = vm.Category.CategoryName;
        //    }

        //    _context.SaveChanges();

        //    return RedirectToAction(nameof(CategoryMaster));
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CategoryMasterVM vm)
        {
            //Console.WriteLine("Selected MainCategoryId = " + vm.Category.MainCategoryId);
            //foreach (var entry in ModelState)
            //{
            //    foreach (var error in entry.Value.Errors)
            //    {
            //        Console.WriteLine($"{entry.Key} → {error.ErrorMessage}");
            //    }
            //}
            if (!ModelState.IsValid)
            {
                vm.Categories = _context.Categories.ToList();
                vm.MainCategoryList = _context.MainCategories
                    .Select(m => new SelectListItem
                    {
                        Text = m.MainCategoryName,
                        Value = m.MainCategoryId.ToString()
                    })
                    .ToList();

                return View("~/Views/Admin/CategoryMaster.cshtml", vm);
            }

            if (vm.Category.CategoryId == 0)
            {
                // INSERT
                _context.Categories.Add(new Category
                {
                    CategoryName = vm.Category.CategoryName,
                    MainCategoryId = vm.Category.MainCategoryId
                });
            }
            else
            {
                // UPDATE
                var existing = _context.Categories
                    .FirstOrDefault(c => c.CategoryId == vm.Category.CategoryId);

                if (existing == null)
                    return NotFound();

                existing.CategoryName = vm.Category.CategoryName;
                existing.MainCategoryId = vm.Category.MainCategoryId;
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
