using AllupBackendProject.DAL;
using AllupBackendProject.Models;
using FrontToBack.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            List<Category> categories = _context.Categories.Include(x => x.BrandCategories).ThenInclude(x => x.ProductBrand).ToList();
            return View(categories);
        }
        public async Task<ActionResult> Active(int? id)
        {
            if (id == null) return NotFound();
            Category dbCategory = await _context.Categories.FindAsync(id);
            if (dbCategory == null) return NotFound();
            dbCategory.IsDeleted = false;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");

        }
        public ActionResult Create()
        {
            List<Category> MainCategory = _context.Categories.Where(x => x.IsMain == true && x.IsDeleted == false).ToList();
            ViewBag.Category = MainCategory;
            return View();
        }
        
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            bool isExist = _context.Categories.Any(c => c.Name.ToLower() == category.Name.ToLower().Trim());

            if (isExist)
            {
                ModelState.AddModelError("Name", "This category is already exists");
                return RedirectToAction("Index");

            }
            if (!category.IsMain)
            {
                Category subCategory = new Category();
                List<Category> db = _context.Categories.Where(c => c.Id == category.MainCategory.Id).ToList();


                subCategory.MainCategory = db.FirstOrDefault();
                subCategory.Name = category.Name;

                if (subCategory.MainCategory == null)
                {

                    return NotFound();

                }
                await _context.Categories.AddAsync(subCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");

            }
            if (ModelState["Photo"].ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
            {
                ModelState.AddModelError("Photo", "Do not empty");
            }

            if (!category.Photo.IsImage())
            {
                ModelState.AddModelError("Image", "only image");
                return RedirectToAction("Index");

            }
            if (category.Photo.IsCorrectSize(300))
            {
                ModelState.AddModelError("Image", "please enter photo under 300kb");
                return RedirectToAction("Index");
            }



            string fileName = await category.Photo.SaveImageAsync(_env.WebRootPath, "images");
            Category mainCategory = new Category
            {
                Name = category.Name,
                IsMain = category.IsMain,
                IsFeatured = category.IsFeatured,
                ImageUrl = fileName,
            };

            await _context.Categories.AddAsync(mainCategory);
            await _context.SaveChangesAsync();



            return RedirectToAction(nameof(Index));

        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            Category dbCategory = await _context.Categories.FindAsync(id);
            if (dbCategory == null) return NotFound();
            return View(dbCategory);
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
        public async Task<ActionResult> Delete(int? id, Category category)
        {
            if (id == null) return NotFound();
            Category dbCategory = await _context.Categories.FindAsync(id);
            if (dbCategory == null) return NotFound();
            if (dbCategory.IsMain)
            {
                bool isSubcategory = _context.Categories.Any(c => c.MainCategory.Id == category.Id);
                if (isSubcategory)
                {
                    dbCategory.IsDeleted = true;
                }
                else
                {
                    string path = Path.Combine(_env.WebRootPath, "images/", dbCategory.ImageUrl);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                    _context.Categories.Remove(dbCategory);

                }
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");

            }


            _context.Categories.Remove(dbCategory);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            Category dbCategory = await _context.Categories.FindAsync(id);
            if (dbCategory == null) return NotFound();

            List<Category> MainCategory = _context.Categories.Where(x => x.IsMain == true && x.IsDeleted == false).ToList();
            ViewBag.Category = MainCategory;

            return View(dbCategory);
        }
        [HttpPost]
        public async Task<ActionResult> Edit(int? id, Category category)
        {
            bool isExist = _context.Categories.Any(c => c.Name.ToLower() == category.Name.ToLower().Trim());
            Category newCategory = await _context.Categories.FindAsync(id);

            if (isExist && !(newCategory.Name.ToLower() == category.Name.ToLower().Trim()))
            {
                ModelState.AddModelError("Name", "There is already has a category with this name");
                return View();
            };

            if (!category.IsMain)
            {
                newCategory.MainCategory = await _context.Categories.FindAsync(category.MainCategory.Id);
                newCategory.Name = category.Name;

            }
            if (category.Photo != null)
            {
                if (ModelState["Photo"].ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
                {
                    ModelState.AddModelError("Photo", "Do not empty");
                }

                if (!category.Photo.IsImage())
                {
                    ModelState.AddModelError("Image", "Please only upload files with image type.");
                    return View();
                }
                if (category.Photo.IsCorrectSize(300))
                {
                    ModelState.AddModelError("Image", "Image size cannot be more than 300");
                    return View();
                }
                string path = Path.Combine(_env.WebRootPath, "images", newCategory.ImageUrl);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                string fileName = await category.Photo.SaveImageAsync(_env.WebRootPath, "images");
                newCategory.IsFeatured = category.IsFeatured;
                newCategory.ImageUrl = fileName;

            }
            newCategory.Name = category.Name;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");

        }

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return NotFound();
            Category dbCategory = await _context.Categories.FindAsync(id);
            if (dbCategory == null) return NotFound();
            return View(dbCategory);
        }
    }
}
