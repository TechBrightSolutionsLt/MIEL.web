using System.Collections.Generic;
using MIEL.web.Models.EntityModels;
using MIEL.web.Repositories;
using MIEL.web.Models; 

namespace MIEL.web.Services
{
    public class CategoryService 
    {
        private readonly ICategoryRepository _repository;

        public CategoryService(ICategoryRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<Category> GetCategories()
        {
            return _repository.GetAll();
        }

        public Category? GetCategoryById(int id)
        {
            return _repository.GetById(id);
        }

        public void CreateCategory(Category category)
        {
            _repository.Add(category);
            _repository.Save();
        }

        public void UpdateCategory(Category category)
        {
            _repository.Update(category);
            _repository.Save();
        }

        public void DeleteCategory(int id)
        {
            _repository.Delete(id);
            _repository.Save();
        }
    }
}
