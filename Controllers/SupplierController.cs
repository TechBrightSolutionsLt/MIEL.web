using Microsoft.AspNetCore.Mvc;
using MIEL.web.Data;
using MIEL.web.Models.EntityModels;
namespace MIEL.web.Controllers
{
    public class SupplierController : Controller
    {
        private readonly AppDBContext _context;

        public SupplierController(AppDBContext context)
        {
            _context = context;
        }

        // CREATE
        public IActionResult Create()
        {
            ViewBag.SupplierList = _context.Suppliers.ToList();
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Supplier supplier)
        {

            if (ModelState.IsValid)
            {

                _context.Suppliers.Add(supplier);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Supplier saved successfully!";
                return RedirectToAction(nameof(Create)); // stay on same page
            }

            ViewBag.SupplierList = _context.Suppliers.ToList();
            return View(supplier);
        }

        // GET: Edit page
        public IActionResult Edit(int id)
        {
            var supplier = _context.Suppliers.Find(id);
            if (supplier == null)
                return NotFound();

            return View(supplier); // this will look for Edit.cshtml
        }

        // POST: Update supplier
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Supplier supplier)
        {
            if (!ModelState.IsValid)
                return View(supplier); // show errors in Edit.cshtml

            _context.Suppliers.Update(supplier);
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Supplier Updated successfully!";
            return RedirectToAction(nameof(Create)); // redirect back to Create page with grid
        }


        // DELETE
        public IActionResult Delete(int id)
        {
            var supplier = _context.Suppliers.Find(id);
            if (supplier != null)
            {
                _context.Suppliers.Remove(supplier);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Supplier deleted successfully!";
            }
            return RedirectToAction(nameof(Create));
        }
    }
}
