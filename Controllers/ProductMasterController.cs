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

            // -------- 1) Save Product first --------
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
            _db.SaveChanges();   // IMPORTANT — generates ProductId

            // -------- 2) Prepare image folder --------
            var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/proimg");
            if (!Directory.Exists(uploadDir))
                Directory.CreateDirectory(uploadDir);

            // -------- 3) Helper method with FLAG --------
            void SaveImage(IFormFile file, int flag)
            {
                if (file != null && file.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(uploadDir, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    //var productImage = new ProductImages
                    //{
                    //    ProductId = product.ProductId,
                    //    ImgPath = "/proimg/" + fileName,
                    //    Flag = flag        // 👉 IMPORTANT
                    //};

                    //_db.ProductImages.Add(productImage);
                }
            }

            // -------- 4) Save 4 images with correct flag --------
            SaveImage(model.Image, 1);   // ✅ MAIN IMAGE
            SaveImage(model.Image2, 0);
            SaveImage(model.Image3, 0);
            SaveImage(model.Image4, 0);

            _db.SaveChanges(); // Save all images together

            TempData["msg"] = "Product and Images Saved Successfully!";
            return RedirectToAction("Index");
        }


    }
}