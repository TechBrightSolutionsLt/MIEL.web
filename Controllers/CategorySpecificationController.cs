using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MIEL.web.Data;
using MIEL.web.Models.ViewModel;

namespace MIEL.web.Controllers
{
    public class CategorySpecificationController : Controller
    {
        private readonly CategoryService _categoryService;

        public CategorySpecificationController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var vm = new CategorySpecificationVM();

            var categories = _categoryService.GetCategories();

            vm.Categories = categories.Select(x => new SelectListItem
            {
                Value = x.CategoryId.ToString(),
                Text = x.Name
            }).ToList();

            return View("~/Views/Admin/CategorySpecification.cshtml", vm);
        }
    }
}
