using Microsoft.EntityFrameworkCore;
using MIEL.web.Models.EntityModels;
using MIEL.web.Models;

namespace MIEL.web.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options)
            : base(options)
        {
        }

        // Existing users table
        public DbSet<userModel> users_TB { get; set; }

        // Add this line for Category table
      
        public DbSet<Category> Categories { get; set; }

        public DbSet<ImageItem> ImageItems { get; set; }

        public DbSet<CategorySpecification> Specifications { get; set; }
        // 👉 THIS LINE MUST EXIST
        public DbSet<ProductMaster> ProductMasters { get; set; }

        public DbSet<ProductImages> ProductImages { get; set; }

        public DbSet<procolrsizevarnt> ProColorSizeVariants { get; set; }

        public DbSet<productspecification> productspecifications { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }


        public DbSet<Customer> Customers { get; set; }
        public DbSet<MainCategory> MainCategories { get; set; }
    }
}
