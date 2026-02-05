using Microsoft.AspNetCore.Mvc;
using MIEL.web.Data;
using MIEL.web.Models.EntityModels;
using MIEL.web.Models.ViewModel;
using System;
using System.Linq;

namespace MIEL.web.Controllers
{
    public class ProductMasterController : Controller
    {
        private readonly AppDBContext _db;

        public ProductMasterController(AppDBContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var model = new ProductMasterVM
            {
                Categories = _db.Categories.ToList()
            };

            return View(model);
        }

        [HttpPost]
        public JsonResult GetSpecifications(int categoryId)
        {
            var specs = _db.Specifications
                            .Where(s => s.CategoryId == categoryId)
                            .Select(s => new
                            {
                                specName = s.SpecName,
                                options = s.Options,
                                optionType = s.OptionType
                            })
                            .ToList();

            return Json(specs);
        }

        [HttpPost]
        public IActionResult SaveProduct(ProductMasterVM model)
        {
            var product = new ProductMaster
            {
                CategoryId = model.SelectedCategoryId,
                ProductCode = model.ProductCode,
                ProductName = model.ProductName,
                Brand = model.Brand,
                ProductDescription = model.ProductDescription,
                Occasion = model.Occasion,
                ComboPackage = model.ComboPackage,
                HSNNo = model.HSNNo,
                CreatedDate = DateTime.Now
            };

            _db.ProductMasters.Add(product);
            _db.SaveChanges();

            TempData["msg"] = "Product Saved Successfully!";
            return RedirectToAction("Index");
        }

    }
}
