using Microsoft.AspNetCore.Mvc;
using MIEL.web.Data;
using MIEL.web.Models.EntityModels;
using MIEL.web.Models.ViewModel;
using System;
using System.IO;
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
            if (!ModelState.IsValid)
                return View("Index", model);

            // Save Product
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

            // Save uploaded image
            if (model.Image != null && model.Image.Length > 0)
            {
                var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/proimg");
                if (!Directory.Exists(uploadDir))
                    Directory.CreateDirectory(uploadDir);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Image.FileName);
                var filePath = Path.Combine(uploadDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.Image.CopyTo(stream);
                }

                // Save image path in ProductImages table
                var productImage = new ProductImage
                {
                    ProductId = product.ProductId,
                    ImgPath = "/proimg/" + fileName
                };

                _db.ProductImages.Add(productImage);
                _db.SaveChanges();
            }

            TempData["msg"] = "Product and Image Saved Successfully!";
            return RedirectToAction("Index");
        }
    }
}
