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
    public class BrandController : Controller
    {
        private readonly Context _context;
        private readonly IWebHostEnvironment _env;

        public BrandController(Context context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            IEnumerable<BrandBanner> brands = _context.Brands.ToList();
            return View(brands);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Create(BrandBanner brand)
        {
            if (ModelState["Photo"].ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
            {
                ModelState.AddModelError("Photo", "Do not empty");
            }
            if (!brand.Photo.IsImage())
            {
                ModelState.AddModelError("Photo", "Please upload only image files");
                return View();  
            }
            if (brand.Photo.IsCorrectSize(300))
            {
                ModelState.AddModelError("Photo", "The photo size cannot be more than 300");
                return View();
            }
            BrandBanner newBrand = new BrandBanner();

            string fileName = await brand.Photo.SaveImageAsync(_env.WebRootPath, "brand");
            newBrand.ImageUrl = fileName;
            await _context.Brands.AddAsync(newBrand);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            BrandBanner dbBrandBanner = await _context.Brands.FindAsync(id);
            if (dbBrandBanner == null) return NotFound();
            return View(dbBrandBanner);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteSlider(int? id)
        {
            if (id == null) return NotFound();
            BrandBanner dbBrandBanner = await _context.Brands.FindAsync(id);
            if (dbBrandBanner == null) return NotFound();

            string path = Path.Combine(_env.WebRootPath, "brand", dbBrandBanner.ImageUrl);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            _context.Brands.Remove(dbBrandBanner);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return NotFound();
            BrandBanner dbBrandBanner = await _context.Brands.FindAsync(id);
            if (dbBrandBanner == null) return NotFound();
            return View(dbBrandBanner);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Update(int? id, BrandBanner brandBanner)
        {
            if (brandBanner.Photo != null)
            {
                if (ModelState["Photo"].ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
                {
                    ModelState.AddModelError("Photo", "Do not empty");
                }
                if (!brandBanner.Photo.IsImage())
                {
                    ModelState.AddModelError("Photo", "Please upload only image files");
                    return View();
                }
                if (brandBanner.Photo.IsCorrectSize(300))
                {
                    ModelState.AddModelError("Photo", "The photo size cannot be more than 300");
                    return View();
                }
                BrandBanner dbBrandBanner = await _context.Brands.FindAsync(id);
                string path = Path.Combine(_env.WebRootPath, "brand", dbBrandBanner.ImageUrl);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                string filename = await brandBanner.Photo.SaveImageAsync(_env.WebRootPath, "brand");
                dbBrandBanner.ImageUrl = filename;

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return NotFound();
            BrandBanner dbBrandBanner = await _context.Brands.FindAsync(id);
            if (dbBrandBanner == null) return NotFound();
            return View(dbBrandBanner);
        }
    }
}
