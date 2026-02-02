using Microsoft.EntityFrameworkCore;

namespace MIEL.web.Data
{
    public class AppDBContext: DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
        }
        
    }
    
}
