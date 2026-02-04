using MIEL.web.Models.EntityModels;
using System.Collections.Generic;
using MIEL.web.Models;

public interface ICategorySpecificationRepository
{
    List<Category> GetAllCategory();
    List<CategorySpecification> GetByCategory(int categoryId);

    void Add(CategorySpecification spec);

    void Update(CategorySpecification spec);

    void Delete(int id);
}
