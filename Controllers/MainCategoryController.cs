using Microsoft.AspNetCore.Mvc;
using MIEL.web.Data;
using MIEL.web.Models.EntityModels;
using System.Linq;

namespace MIEL.web.Controllers
{
    public class MainCategoryController : Controller
    {
        private readonly AppDBContext _context;

        public MainCategoryController(AppDBContext context)
        {
            _context = context;
        }

        // Redirect Index → Create (because no Index view)
        public IActionResult Index()
        {
            return RedirectToAction("Create");
        }

        // CREATE + LIST (GET)
        public IActionResult Create()
        {
            ViewBag.Categories = _context.MainCategories.ToList();
            return View();
        }

        // CREATE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(MainCategory model)
        {
            if (ModelState.IsValid)
            {
                _context.MainCategories.Add(model);
                _context.SaveChanges();
                TempData["SuccessMessage"] = " Main Category saved successfully!";
                return RedirectToAction("Create");
            }

            // reload list if validation fails
            ViewBag.Categories = _context.MainCategories.ToList();
            return View(model);
        }

        // EDIT (GET)
        public IActionResult Edit(int id)
        {
            var category = _context.MainCategories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // EDIT (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(MainCategory model)
        {
            if (ModelState.IsValid)
            {
                _context.MainCategories.Update(model);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Updated Successfully!";
                return RedirectToAction("Create");
            }
            return View(model);
        }

        // DELETE (NO VIEW)
        public IActionResult Delete(int id)
        {
            var category = _context.MainCategories.Find(id);
            if (category != null)
            {
                _context.MainCategories.Remove(category);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Deleted successfully!";
            }
            return RedirectToAction("Create");
        }
    }
}
