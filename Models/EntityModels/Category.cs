using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using MIEL.web.Models.EntityModels;
using System.ComponentModel.DataAnnotations;

public class Category
{
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Category name is required")]
    public string CategoryName { get; set; }

    public int? MainCategoryId { get; set; }

    [ValidateNever] // ⭐⭐⭐ THIS FIXES ModelState
    public MainCategory MainCategory { get; set; }
}