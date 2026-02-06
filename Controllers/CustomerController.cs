using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MIEL.web.Data;
using MIEL.web.Models.EntityModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace MIEL.web.Controllers
{
    public class CustomerController : Controller
    {
        private readonly AppDBContext _context;

        public CustomerController(AppDBContext context)
        {
            _context = context;
        }

        // ================= INDEX =================
        public IActionResult Index()
        {
            // Simply list all customers
            var customers = _context.Customers.ToList();
            return View(customers);
        }

        // ================= CREATE =================
        // GET: Create
        public IActionResult Create()
        {
            // Load states for dropdown
            //ViewBag.States = GetStates();

            // Load customers for grid
            ViewBag.CustomerList = _context.Customers.ToList();
            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Customers.Add(customer);
                _context.SaveChanges();

                // Success message
                TempData["SuccessMessage"] = "Customer created successfully!";

                // Redirect back to Create page to show grid and modal
                return RedirectToAction(nameof(Create));
            }

            // Reload states and grid if validation fails
            ////ViewBag.States = GetStates();
            ViewBag.CustomerList = _context.Customers.ToList();
            return View(customer);
        }

        // ================= EDIT =================
        // GET: Edit
        public IActionResult Edit(int id)
        {
            var customer = _context.Customers.Find(id);
            if (customer == null)
                return NotFound();

            ////ViewBag.States = GetStates();
            return View(customer);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                // ❗ Stay on Edit page and show validation errors
                return View(customer);
            }

            _context.Customers.Update(customer);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Customer updated successfully!";
            return RedirectToAction(nameof(Create));
        }

        // Delete
        public IActionResult Delete(int id)
        {
            var supplier = _context.Customers.Find(id);
            if (supplier != null)
            {
                _context.Customers.Remove(supplier);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Supplier deleted successfully!";
            }
            return RedirectToAction(nameof(Create));
        }

        // ================= HELPER =================
        //private List<string> GetStates()
        //{
        //    return new List<string>
        //    {
        //        "New South Wales",
        //        "Victoria",
        //        "Queensland",
        //        "Western Australia",
        //        "South Australia",
        //        "Tasmania",
        //        "Australian Capital Territory",
        //        "Northern Territory"
        //    };
        //}
    }
}
