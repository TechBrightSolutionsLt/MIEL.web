using Microsoft.AspNetCore.Mvc;
using MIEL.web.Data;
using MIEL.web.Models.ViewModels;
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
            var model = new CategoryMasterVM
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

    }
}
