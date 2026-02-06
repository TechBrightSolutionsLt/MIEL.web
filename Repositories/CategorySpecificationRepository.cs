using MIEL.web.Data;
using MIEL.web.Models.EntityModels;
using System.Collections.Generic;
using System.Linq;

public class CategorySpecificationRepository : ICategorySpecificationRepository
{
    private readonly AppDBContext _context;

    public CategorySpecificationRepository(AppDBContext context)
    {
        _context = context;
    }

    public List<Category> GetAllCategory()
    {
        return _context.Categories.ToList();
    }
    public List<CategorySpecification> GetByCategory(int categoryId)
    {
        return _context.Specifications
                       .Where(x => x.CategoryId == categoryId)
                       .ToList();
    }

    public void Add(CategorySpecification spec)
    {
        if (string.IsNullOrWhiteSpace(spec.SpecName))
            throw new Exception("SpecName cannot be empty");

        _context.Specifications.Add(spec);
        _context.SaveChanges();
    }


    public void Update(CategorySpecification spec)
    {
        var data = _context.Specifications
                           .FirstOrDefault(x => x.Id == spec.Id);

        if (data != null)
        {
            data.SpecName = spec.SpecName;
            _context.SaveChanges();
        }
    }
    public void Delete(int id)
    {
        var data = _context.Specifications
                           .FirstOrDefault(x => x.Id == id);

        if (data != null)
        {
            _context.Specifications.Remove(data);
            _context.SaveChanges();
        }
    }
  
}
