using Microsoft.AspNetCore.Mvc;
using MIEL.web.Data;
using MIEL.web.Models.EntityModels;

namespace MIEL.web.Controllers
{
    public class ImageController : Controller
    {
        private readonly AppDBContext _context;
        private readonly IWebHostEnvironment _env;

        public ImageController(AppDBContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index()
        {
            return View(_context.ImageItems.ToList());
        }

        [HttpPost]
        public IActionResult Upload(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return RedirectToAction(nameof(Index));

            string uploads = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploads);

            string fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
            string filePath = Path.Combine(uploads, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            imageFile.CopyTo(stream);

            _context.ImageItems.Add(new ImageItem
            {
                ImagePath = "/uploads/" + fileName
            });

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var image = _context.ImageItems.Find(id);
            if (image != null)
            {
                string path = Path.Combine(_env.WebRootPath, image.ImagePath.TrimStart('/'));
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);

                _context.ImageItems.Remove(image);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}