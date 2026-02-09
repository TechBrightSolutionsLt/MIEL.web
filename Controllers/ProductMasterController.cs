using Microsoft.AspNetCore.Http;
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
                TempData["SuccessMessage"] = "Product Created successfully!";



                // ---------- 2. IMAGE FOLDER ----------
                var uploadDir = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/proimg"
                );

                if (!Directory.Exists(uploadDir))
                    Directory.CreateDirectory(uploadDir);

                // ---------- SAVE SIZE CHART ----------
                var sizeChartFile = Request.Form.Files["SizeChartImg"];

                if (sizeChartFile != null && sizeChartFile.Length > 0)
                {
                    var sizeChartDir = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot/sizecharts"
                    );

                    if (!Directory.Exists(sizeChartDir))
                        Directory.CreateDirectory(sizeChartDir);

                    var fileName = Guid.NewGuid() + Path.GetExtension(sizeChartFile.FileName);
                    var path = Path.Combine(sizeChartDir, fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        sizeChartFile.CopyTo(stream);
                    }

                    product.sizechartPath = "/sizecharts/" + fileName;
                    _db.SaveChanges();
                }



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


                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.InnerException?.Message ?? ex.Message;
                return RedirectToAction("Index");
            }
        }

        // ================= LIST =================
        public IActionResult List()
        {
            var products = _db.ProductMasters
                .Select(p => new
                {
                    p.ProductId,
                    p.ProductName,
                    CategoryName = _db.Categories
                        .Where(c => c.CategoryId == p.CategoryId)
                        .Select(c => c.CategoryName)
                        .FirstOrDefault(),
                    p.Brand,
                    p.CreatedDate,
                    p.sizechartPath
                })
                .ToList();

            return View(products);
        }

        // ================= EDIT =================
        public IActionResult Edit(int id)
        {
            var vm = new ProductEditVM
            {
                Product = _db.ProductMasters.First(p => p.ProductId == id),
                Categories = _db.Categories.ToList(),
                Specifications = _db.productspecifications
                                    .Where(s => s.ProductId == id)
                                    .ToList(),
                Variants = _db.ProColorSizeVariants
                                    .Where(v => v.ProductId == id)
                                    .ToList(),
                Images = _db.ProductImages
                    .Where(i => i.ProductId == id)
                    .OrderByDescending(i => i.Flag)
                    .ToList()


            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult Update()
        {
            int productId = Convert.ToInt32(Request.Form["ProductId"]);

            // ---------- 1. UPDATE PRODUCT ----------
            var product = _db.ProductMasters.First(p => p.ProductId == productId);

            product.ProductName = Request.Form["ProductName"];
            product.CategoryId = Convert.ToInt32(Request.Form["CategoryId"]);

            _db.SaveChanges();
            TempData["SuccessMessage"] = "Product Updated successfully!";


            // =====================================================
            // 🔥 IMAGE UPDATE CODE GOES HERE (YOUR CODE)
            // =====================================================

            var uploadDir = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot/proimg"
            );

            if (!Directory.Exists(uploadDir))
                Directory.CreateDirectory(uploadDir);

            // ---------- MAIN IMAGE ----------
            var mainFile = Request.Form.Files["Image"];
            if (mainFile != null && mainFile.Length > 0)
            {
                var oldMainId = Convert.ToInt32(Request.Form["OldMainImageId"]);
                var oldMain = _db.ProductImages
                                 .First(x => x.ImgId == oldMainId);

                var fileName = Guid.NewGuid() + Path.GetExtension(mainFile.FileName);
                var path = Path.Combine(uploadDir, fileName);

                using var stream = new FileStream(path, FileMode.Create);
                mainFile.CopyTo(stream);

                oldMain.ImgPath = "/proimg/" + fileName;
            }

            _db.SaveChanges();


            // ---------- UPDATE SIZE CHART ----------
            var sizeChartFile = Request.Form.Files["SizeChartImg"];

            if (sizeChartFile != null && sizeChartFile.Length > 0)
            {
                var sizeChartDir = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/sizecharts"
                );

                if (!Directory.Exists(sizeChartDir))
                    Directory.CreateDirectory(sizeChartDir);

                // delete old file
                if (!string.IsNullOrEmpty(product.sizechartPath))
                {
                    var oldPath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        product.sizechartPath.TrimStart('/')
                    );

                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                var fileName = Guid.NewGuid() + Path.GetExtension(sizeChartFile.FileName);
                var path = Path.Combine(sizeChartDir, fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    sizeChartFile.CopyTo(stream);
                }

                product.sizechartPath = "/sizecharts/" + fileName;
                _db.SaveChanges();
            }

            // ---------- 2. UPDATE SPECIFICATIONS ----------
            var specs = Request.Form
                .Where(x => x.Key.StartsWith("Specs["))
                .ToList();

            foreach (var spec in specs)
            {
                int specId = int.Parse(
                    spec.Key.Replace("Specs[", "").Replace("]", "")
                );

                var ps = _db.productspecifications
                    .FirstOrDefault(x => x.ProductId == productId && x.Id == specId);

                if (ps != null)
                {
                    ps.specificationvalue = spec.Value;
                }
            }

            _db.SaveChanges();

            // ---------- 3. UPDATE COLOR–SIZE VARIANTS ----------
            // Remove existing variants
            var oldVariants = _db.ProColorSizeVariants
                .Where(v => v.ProductId == productId)
                .ToList();

            _db.ProColorSizeVariants.RemoveRange(oldVariants);
            _db.SaveChanges();

            // Insert new variants
            var colours = Request.Form["colour[]"];
            var sizes = Request.Form["size[]"];

            for (int i = 0; i < colours.Count; i++)
            {
                _db.ProColorSizeVariants.Add(new procolrsizevarnt
                {
                    ProductId = productId,
                    colour = colours[i],
                    size = sizes[i]
                });
            }

            _db.SaveChanges();

            return RedirectToAction("List");
        }


        // ================= DELETE =================
        public IActionResult Delete(int id)
        {
            var product = _db.ProductMasters.FirstOrDefault(p => p.ProductId == id);
            if (product == null)
                return NotFound();

            // ---------- DELETE IMAGES (DB + FILES) ----------
            var images = _db.ProductImages.Where(i => i.ProductId == id).ToList();

            foreach (var img in images)
            {
                var fullPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    img.ImgPath.TrimStart('/')
                );

                if (System.IO.File.Exists(fullPath))
                    System.IO.File.Delete(fullPath);
            }

            _db.ProductImages.RemoveRange(images);

            // ---------- DELETE VARIANTS ----------
            var variants = _db.ProColorSizeVariants
                .Where(v => v.ProductId == id)
                .ToList();
            _db.ProColorSizeVariants.RemoveRange(variants);

            // ---------- DELETE SPECIFICATIONS ----------
            var specs = _db.productspecifications
                .Where(s => s.ProductId == id)
                .ToList();
            _db.productspecifications.RemoveRange(specs);

            // ---------- DELETE PRODUCT ----------
            _db.ProductMasters.Remove(product);

            _db.SaveChanges();

            TempData["SuccessMessage"] = "Product Deleted successfully!";
            return RedirectToAction("List");
        }





    }
}
