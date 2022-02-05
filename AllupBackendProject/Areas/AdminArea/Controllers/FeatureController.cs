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
    public class FeatureController : Controller
    {
        private readonly Context _context;
        private readonly IWebHostEnvironment _env;

        public FeatureController(Context context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            IEnumerable<Feature> features = _context.Features.ToList();
            return View(features);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Create(Feature feature)
        {
            if (ModelState["Photo"].ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
            {
                ModelState.AddModelError("Photo", "Do not empty");
            }
            if (!feature.Photo.IsImage())
            {
                ModelState.AddModelError("Photo", "Please upload only image files");
                return View();
            }
            if (feature.Photo.IsCorrectSize(300))
            {
                ModelState.AddModelError("Photo", "The photo size cannot be more than 300");
                return View();
            }
            Feature newFeature = new Feature();

            string fileName = await feature.Photo.SaveImageAsync(_env.WebRootPath, "banner-icon");
            newFeature.ImageUrl = fileName;
            newFeature.BannerText = feature.BannerText;
            newFeature.BannerTitle = feature.BannerTitle;
            await _context.Features.AddAsync(newFeature);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            Feature dbFeature = await _context.Features.FindAsync(id);
            if (dbFeature == null) return NotFound();
            return View(dbFeature);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteSlider(int? id)
        {
            if (id == null) return NotFound();
            Feature dbFeature = await _context.Features.FindAsync(id);
            if (dbFeature == null) return NotFound();

            string path = Path.Combine(_env.WebRootPath, "banner-icon", dbFeature.ImageUrl);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            _context.Features.Remove(dbFeature);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return NotFound();
            Feature dbFeature = await _context.Features.FindAsync(id);
            if (dbFeature == null) return NotFound();
            return View(dbFeature);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Update(int? id, Feature feature)
        {
            if (feature.Photo != null)
            {
                if (ModelState["Photo"].ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
                {
                    ModelState.AddModelError("Photo", "Do not empty");
                }
                if (!feature.Photo.IsImage())
                {
                    ModelState.AddModelError("Photo", "Please upload only image files");
                    return View();
                }
                if (feature.Photo.IsCorrectSize(300))
                {
                    ModelState.AddModelError("Photo", "The photo size cannot be more than 300");
                    return View();
                }
                Feature dbFeature = await _context.Features.FindAsync(id);
                string path = Path.Combine(_env.WebRootPath, "banner-icon", dbFeature.ImageUrl);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                string filename = await feature.Photo.SaveImageAsync(_env.WebRootPath, "banner-icon");
                dbFeature.ImageUrl = filename;
                dbFeature.BannerTitle = feature.BannerTitle;
                dbFeature.BannerText = feature.BannerText;

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return NotFound();
            Feature dbFeature = await _context.Features.FindAsync(id);
            if (dbFeature == null) return NotFound();
            return View(dbFeature);
        }
    }
}

