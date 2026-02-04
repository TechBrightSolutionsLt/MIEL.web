using Microsoft.EntityFrameworkCore;
using MIEL.web.Models.EntityModels;
using YourNamespace.Models;

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
        public DbSet<userModel> users_TB { get; set; }
        public DbSet<CategorySpecification> Specifications { get; set; }
    }
}
