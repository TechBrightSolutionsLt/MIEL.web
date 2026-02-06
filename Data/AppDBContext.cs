using Microsoft.EntityFrameworkCore;
using MIEL.web.Models;
using MIEL.web.Models.EntityModels;

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
        public DbSet<Product> Products_TB { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<ImageItem> ImageItems { get; set; }

        public DbSet<CategorySpecification> Specifications { get; set; }

        public DbSet<Supplier> Suppliers { get; set; }

    }
}
