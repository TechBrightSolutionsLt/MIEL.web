using Microsoft.EntityFrameworkCore;
using MIEL.web.Models.EntityModels;

namespace MIEL.web.Data
{
    public class AppDBContext: DbContext
    {
    
        public AppDBContext(DbContextOptions<AppDBContext> options)
          : base(options)
        {
        }
        

        public DbSet<userModel> users_TB { get; set; }
    }
    
}
