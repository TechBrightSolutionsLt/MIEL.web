using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MIEL.web.Data;
using MIEL.web.Models.EntityModels;

public class ProductImageController : Controller
{
    private readonly AppDBContext _context;
    private readonly IWebHostEnvironment _env;

    public ProductImageController(AppDBContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    // ==============================
    // 1. PRODUCT LIST WITH SEARCH
    // ==============================
    public IActionResult Index(string search)
    {
        var products = _context.ProductMasters.AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            products = products.Where(p => p.ProductName.Contains(search));
        }

        return View(products.ToList());
    }

    // ==============================
    // 2. SHOW COLORS OF PRODUCT
    // ==============================
    public IActionResult Manage(int id)
    {
        var variants = _context.ProColorSizeVariants
            .Include(v => v.ProdColImages)
            .Where(v => v.ProductId == id)
            .ToList();

        ViewBag.ProductId = id;
        return View(variants);
    }

    // ==============================
    // 3. UPLOAD IMAGES
    // ==============================
    [HttpPost]
    public async Task<IActionResult> Upload(int variantId, List<IFormFile> files)
    {
        string folder = Path.Combine(_env.WebRootPath, "productimages");

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        foreach (var file in files)
        {
            string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            string path = Path.Combine(folder, fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            ProdColImage image = new ProdColImage
            {
                VariantId = variantId,
                ImagePath = fileName
            };

            _context.ProdColImages.Add(image);
        }

        await _context.SaveChangesAsync();

        var variant = _context.ProColorSizeVariants
            .FirstOrDefault(v => v.varientid == variantId);

        return RedirectToAction("Manage", new { id = variant.ProductId });
    }

    // ==============================
    // 4. DELETE IMAGE
    // ==============================
    public async Task<IActionResult> DeleteImage(int id)
    {
        var image = await _context.ProdColImages.FindAsync(id);

        if (image != null)
        {
            string path = Path.Combine(_env.WebRootPath, "productimages", image.ImagePath);

            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);

            _context.ProdColImages.Remove(image);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Manage", new { id = image.Variant.ProductId });
    }

    // ==============================
    // 5. EDIT (REPLACE IMAGE)
    // ==============================
    [HttpPost]
    public async Task<IActionResult> EditImage(int id, IFormFile newFile)
    {
        var image = await _context.ProdColImages.FindAsync(id);

        if (image != null && newFile != null)
        {
            string folder = Path.Combine(_env.WebRootPath, "productimages");

            string oldPath = Path.Combine(folder, image.ImagePath);
            if (System.IO.File.Exists(oldPath))
                System.IO.File.Delete(oldPath);

            string newFileName = Guid.NewGuid() + Path.GetExtension(newFile.FileName);
            string newPath = Path.Combine(folder, newFileName);

            using (var stream = new FileStream(newPath, FileMode.Create))
            {
                await newFile.CopyToAsync(stream);
            }

            image.ImagePath = newFileName;
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Manage", new { id = image.Variant.ProductId });
    }
}