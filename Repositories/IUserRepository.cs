using MIEL.web.Models.EntityModels;

namespace MIEL.web.Repositories
{
    public interface IUserRepository
    {
        void Insert(userModel user);
    }
}
