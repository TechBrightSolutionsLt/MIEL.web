using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MIEL.web.Data;
using MIEL.web.Models.EntityModels;
using MIEL.web.Models.ViewModel;

namespace MIEL.web.Controllers
{
    public class CategorySpecificationController : Controller
    {
        private readonly CategorySpecifications _categoryService;

        public CategorySpecificationController(CategorySpecifications categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var vm = new CategorySpecificationVM();

            var categories = _categoryService.GetAllCategory();

            vm.Categories = categories.Select(x => new SelectListItem
            {
                Value = x.CategoryId.ToString(),
                Text = x.CategoryName
            }).ToList();

            return View("~/Views/Admin/CategorySpecification.cshtml", vm);
        }

        [HttpGet]
        public IActionResult GetSpecifications(int categoryId)
        {
            var specs = _categoryService.GetSpecifications(categoryId);

            var vmList = specs.Select(x => new CategorySpecificationVM
            {
                Id = x.Id,
                SpecName = x.SpecName,
                SelectedCategoryId = x.CategoryId
            }).ToList();

            return Json(vmList);
        }


        [HttpPost]
        public IActionResult SaveSpecification([FromBody] CategorySpecificationVM model)
        {
            if (string.IsNullOrWhiteSpace(model.SpecName))
                return Json(false);

            var entity = new CategorySpecification
            {
                Id = model.Id,
                SpecName = model.SpecName,
                CategoryId = model.SelectedCategoryId
            };

            _categoryService.SaveSpecification(entity);
            return Json(true);
        }

        [HttpPost]
        public IActionResult DeleteSpecification(int id)
        {
            _categoryService.DeleteSpecification(id);
            return Json(true);
        }
    }
}
