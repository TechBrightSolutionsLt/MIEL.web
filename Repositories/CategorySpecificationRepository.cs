using MIEL.web.Data;
using MIEL.web.Models.EntityModels;
using System.Collections.Generic;
using System.Linq;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDBContext _context;

    public CategoryRepository(AppDBContext context)
    {
        _context = context;
    }

    public List<Category> GetAllCategory()
    {
        return _context.Categories.ToList();
    }
}
