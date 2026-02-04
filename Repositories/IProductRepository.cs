using MIEL.web.Models.EntityModels;

namespace MIEL.web.Repositories
{
    public interface IProductRepository
    {
        void Add(Product product);
        List<Product> GetAll();
    }

}
