using MIEL.web.Models.EntityModels;
using MIEL.web.Data;

namespace MIEL.web.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDBContext _context;

        public UserRepository(AppDBContext context)
        {
            _context = context;
        }

        public void Insert(userModel user)
        {
            _context.users_TB.Add(user);
            _context.SaveChanges();
        }
    }
}
