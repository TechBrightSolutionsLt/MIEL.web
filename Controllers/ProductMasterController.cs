using Microsoft.AspNetCore.Mvc;
using MIEL.web.Data;
using MIEL.web.Models.EntityModels;
using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace MIEL.web.Controllers
{
    public class ProductMasterController : Controller
    {
        private readonly AppDBContext _db;

        public ProductMasterController(AppDBContext db)
        {
            _db = db;
        }

        // ================= INDEX =================
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetAll()
        {
            var cats = _db.Categories
                .Select(c => new {
                    categoryId = c.CategoryId,
                    categoryName = c.CategoryName
                })
                .ToList();

            return Json(cats);
        }


        // ================= LOAD CATEGORY SPECS =================
        [HttpPost]
        public JsonResult GetSpecifications(int categoryId)
        {
            var specs = _db.Specifications
                .Where(s => s.CategoryId == categoryId)
                .Select(s => new
                {
                    id = s.Id,                 // IMPORTANT
                    specName = s.SpecName,
                    optionType = s.OptionType,
                    options = s.Options
                })
                .ToList();

            return Json(specs);
        }

        // ================= SAVE PRODUCT (MAIN SAVE BUTTON) =================
        [HttpPost]
        public IActionResult SaveProduct()
        {
            try
            {
                // ---------- 1. SAVE PRODUCT ----------
                var product = new ProductMaster
                {
                    CategoryId = Convert.ToInt32(Request.Form["SelectedCategoryId"]),
                    ProductCode = Request.Form["ProductCode"],
                    ProductName = Request.Form["ProductName"],
                    Brand = Request.Form["Brand"],
                    ProductDescription = Request.Form["ProductDescription"],
                    Occasion = Request.Form["Occasion"],
                    ComboPackage = Request.Form["ComboPackage"],
                    HSNNo = Request.Form["HSNNo"],
                    BarcodeNo = Request.Form["BarcodeNo"],
                    CreatedDate = DateTime.Now
                };

                _db.ProductMasters.Add(product);
                _db.SaveChanges(); // 🔑 ProductId generated


                // ---------- 2. IMAGE FOLDER ----------
                var uploadDir = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/proimg"
                );

                if (!Directory.Exists(uploadDir))
                    Directory.CreateDirectory(uploadDir);


                // ---------- 3. SAVE PRODUCT IMAGES ----------
                void SaveImage(IFormFile file, int flag)
                {
                    if (file != null && file.Length > 0)
                    {
                        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                        var path = Path.Combine(uploadDir, fileName);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        _db.ProductImages.Add(new ProductImages
                        {
                            ProductId = product.ProductId,
                            ImgPath = "/proimg/" + fileName,
                            Flag = flag
                        });
                    }
                }

                SaveImage(Request.Form.Files["Image"], 1);   // MAIN
                SaveImage(Request.Form.Files["Image2"], 0);
                SaveImage(Request.Form.Files["Image3"], 0);
                SaveImage(Request.Form.Files["Image4"], 0);

                _db.SaveChanges();


                // ---------- 4. SAVE COLOR & SIZE VARIANTS ----------
                var colours = Request.Form["colour[]"];
                var sizes = Request.Form["size[]"];

                for (int i = 0; i < colours.Count; i++)
                {
                    _db.ProColorSizeVariants.Add(new procolrsizevarnt
                    {
                        ProductId = product.ProductId,
                        colour = colours[i],
                        size = sizes[i]
                    });
                }

                _db.SaveChanges();


                // ---------- 5. SAVE PRODUCT SPECIFICATIONS ----------
                var specs = Request.Form
                    .Where(x => x.Key.StartsWith("Specs["))
                    .ToList();

                foreach (var spec in specs)
                {
                    var specIdStr = spec.Key
                        .Replace("Specs[", "")
                        .Replace("]", "");

                    if (!int.TryParse(specIdStr, out int specId))
                        continue;

                    _db.productspecifications.Add(new productspecification
                    {
                        ProductId = product.ProductId,
                        Id = specId,                    // FK → Specifications.Id
                        specificationvalue = spec.Value
                    });
                }

                _db.SaveChanges();


                TempData["msg"] = "Product saved successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Error: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}
