using MIEL.web.Models.EntityModels;
using MIEL.web.Models.ViewModel;
using MIEL.web.Repositories;

namespace MIEL.web.Services
{
    public class ProductService
    {
        private readonly IProductRepository _productRepo;

        public ProductService(IProductRepository productRepo)
        {
            _productRepo = productRepo;
        }

        public void CreateProduct(ProductCreateViewModel vm)
        {
            // convert checkbox values → CSV
            if (vm.SelectedSizes != null && vm.SelectedSizes.Any())
            {
                vm.Product.AvailableSize = string.Join(",", vm.SelectedSizes);
            }

            _productRepo.Add(vm.Product);
        }


        public List<Product> GetAll()
        {
            return _productRepo.GetAll();
        }
    }
}
