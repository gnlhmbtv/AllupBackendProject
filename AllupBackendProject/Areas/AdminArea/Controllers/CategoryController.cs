using AllupBackendProject.DAL;
using AllupBackendProject.Models;
using FrontToBack.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AllupBackendProject.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    public class CategoryController : Controller
    {
        private readonly Context _context;
        private readonly IWebHostEnvironment _env;
        public CategoryController(Context context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            IEnumerable<Category> categories = _context.Categories.ToList();
            return View(categories);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid) return NotFound();
            bool isExist = _context.Categories.Any(c => c.Name.ToLower().Trim() == category.Name.ToLower().Trim());
            if (isExist)
            {
                ModelState.AddModelError("Name", "There is already have this category");
            }
            if (!category.Photo.IsImage())
            {
                ModelState.AddModelError("Photo", "Please upload only image files");
                return View();
            }
            if (category.Photo.IsCorrectSize(300))
            {
                ModelState.AddModelError("Photo", "The photo size cannot be more than 300");
                return View();
            }

            string fileName = await category.Photo.SaveImageAsync(_env.WebRootPath, "images");
            Category newCategory = new Category()
            {
                Name = category.Name,
                ImageUrl = fileName
            };

            await _context.Categories.AddAsync(newCategory);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            Category dbCategory = await _context.Categories.FindAsync(id);
            if (dbCategory == null) return NotFound();
            return View(dbCategory);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteCategory(int? id)
        {
            if (id == null) return NotFound();
            Category dbCategory = await _context.Categories.FindAsync(id);
            if (dbCategory == null) return NotFound();

            string path = Path.Combine(_env.WebRootPath, "images", dbCategory.ImageUrl);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            _context.Categories.Remove(dbCategory);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return NotFound();
            Category dbCategory = await _context.Categories.FindAsync(id);
            if (dbCategory == null) return NotFound();
            return View(dbCategory);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int? id, Category category)
        {
            if (!ModelState.IsValid) return View();
            bool isExist = _context.Categories.Any(c => c.Name.ToLower() == category.Name.ToLower().Trim());

            Category isExistCategory = _context.Categories.FirstOrDefault(c => c.Id == id);

            if (isExist && !(isExistCategory.Name.ToLower() == category.Name.ToLower().Trim()))
            {
                ModelState.AddModelError("Name", "There is already have a category with this name.");
                return View();
            };
                if (!category.Photo.IsImage())
                {
                    ModelState.AddModelError("Photo", "Please upload only image files");
                    return View();
                }
                if (category.Photo.IsCorrectSize(300))
                {
                    ModelState.AddModelError("Photo", "The photo size cannot be more than 300");
                    return View();
                }
                Category dbCategory = await _context.Categories.FindAsync(id);
                string path = Path.Combine(_env.WebRootPath, "images", dbCategory.ImageUrl);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                string filename = await category.Photo.SaveImageAsync(_env.WebRootPath, "images");

                dbCategory.ImageUrl = filename;
                dbCategory.Name = category.Name;

                await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }
    }
}
