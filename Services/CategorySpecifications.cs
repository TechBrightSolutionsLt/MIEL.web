using MIEL.web.Models.EntityModels;
using MIEL.web.Repositories;
using System.Collections.Generic;
using YourNamespace.Models;

public class CategorySpecifications
{
    private readonly ICategorySpecificationRepository _repo;

    public CategorySpecifications(ICategorySpecificationRepository repo)
    {
        _repo = repo;
    }

    public List<Category> GetAllCategory()
    {
        return _repo.GetAllCategory();
    }
    public List<CategorySpecification> GetSpecifications(int categoryId)
    {
        return _repo.GetByCategory(categoryId);
    }

    public void SaveSpecification(CategorySpecification spec)
    {
        if (spec.Id > 0)
        {
            _repo.Update(spec);   // UPDATE
        }
        else
        {
            _repo.Add(spec);      // INSERT
        }
    }

    public void DeleteSpecification(int id)
    {
        _repo.Delete(id);
    }
}
