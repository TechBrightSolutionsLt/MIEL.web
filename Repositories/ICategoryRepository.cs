using System.Collections.Generic;
using MIEL.web.Models.EntityModels;
using MIEL.web.Models;

namespace MIEL.web.Repositories
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> GetAll();
        Category? GetById(int id);
        void Add(Category category);
        void Update(Category category);
        void Delete(int id);
        void Save();
       
    }
}
