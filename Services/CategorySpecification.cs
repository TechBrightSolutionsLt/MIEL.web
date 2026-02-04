using MIEL.web.Models.Entities;
using MIEL.web.Repositories;
using System.Collections.Generic;

public class CategoryService
{
    private readonly ICategoryRepository _repo;

    public CategoryService(ICategoryRepository repo)
    {
        _repo = repo;
    }

    public List<Category> GetAllCategory()
    {
        return _repo.GetAll();
    }
}
