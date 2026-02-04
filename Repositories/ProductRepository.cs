using Microsoft.EntityFrameworkCore;
using MIEL.web.Data;
using MIEL.web.Models.EntityModels;

namespace MIEL.web.Repositories
{
  
        public class ProductRepository : IProductRepository
        {
            private readonly AppDBContext _context;

            public ProductRepository(AppDBContext context)
            {
                _context = context;
            }

            public void Add(Product product)
            {
                _context.Products_TB.Add(product);
                _context.SaveChanges();
            }

            public List<Product> GetAll()
            {
                return _context.Products_TB.Include(p => p.Category).ToList();
            }
        }
}
