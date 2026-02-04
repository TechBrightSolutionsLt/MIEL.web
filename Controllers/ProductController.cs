using Microsoft.AspNetCore.Mvc;
using MIEL.web.Models.EntityModels;
using MIEL.web.Models.ViewModel;
using MIEL.web.Repositories;
using MIEL.web.Services;

namespace MIEL.web.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductService _productService;
        private readonly ICategoryRepository _categoryRepo;
        //private readonly CategorySpecification _specRepo;
        private readonly ICategorySpecificationRepository _specRepo;

        public ProductController(
            ProductService productService,
            ICategoryRepository categoryRepo,
            ICategorySpecificationRepository specRepo)
        {
            _productService = productService;
            _categoryRepo = categoryRepo;
            _specRepo = specRepo;
        }

        // GET
        public IActionResult Create()
        {
            var vm = new ProductCreateViewModel
            {
                Product = new Product(),
                Categories = _categoryRepo.GetALL(),
                Specifications = _specRepo.GetAll()
            };

            return View(vm);
        }

        [HttpGet]
        public IActionResult GetSizesByCategory(int categoryId)
        {
            var sizes = _specRepo
                .GetByCategory(categoryId)
                .FirstOrDefault(x => x.SpecName.ToLower() == "size");

            if (sizes == null || string.IsNullOrEmpty(sizes.Options))
                return Json(new List<string>());

            var options = sizes.Options.Split(',').ToList();
            return Json(options);
        }


        // POST
        [HttpPost]
        public IActionResult Create(ProductCreateViewModel vm)
        {
            _productService.CreateProduct(vm);
            return RedirectToAction("Index");
        }
    }

}
