using AllupBackendProject.DAL;
using AllupBackendProject.Models;
using FrontToBack.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AllupBackendProject.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    public class SliderController : Controller
    {
        private readonly Context _context;
        private readonly IWebHostEnvironment _env;
        public SliderController(Context context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            List<Slider> sliders = _context.Sliders.ToList();
            return View(sliders);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Slider slider)
        {
            if (ModelState["Photos"].ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
            {
                ModelState.AddModelError("Photos", "Do not empty");
            }
            if (!slider.Photos.IsImage())
            {
                ModelState.AddModelError("Photo", "Please upload only image files");
                return View();
            }
            if (slider.Photos.IsCorrectSize(300))
            {
                ModelState.AddModelError("Photo", "The photo size cannot be more than 300");
                return View();
            }
            Slider newSlider = new Slider();

            string fileName = await slider.Photos.SaveImageAsync(_env.WebRootPath, "images");
            newSlider.ImageUrl = fileName;
            newSlider.MainTitle = slider.MainTitle;
            newSlider.SubTitle = slider.SubTitle;
            newSlider.SliderText = slider.SliderText;
            await _context.Sliders.AddAsync(newSlider);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            Slider dbSlider = await _context.Sliders.FindAsync(id);
            if (dbSlider == null) return NotFound();
            return View(dbSlider);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteSlider(int? id)
        {
            if (id == null) return NotFound();
            Slider dbSlider = await _context.Sliders.FindAsync(id);
            if (dbSlider == null) return NotFound();

            string path = Path.Combine(_env.WebRootPath, "image", dbSlider.ImageUrl);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            _context.Sliders.Remove(dbSlider);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return NotFound();
            Slider dbSlider = await _context.Sliders.FindAsync(id);
            if (dbSlider == null) return NotFound();
            return View(dbSlider);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Slider slider)
        {
            if (slider.Photos != null)
            {
                if (ModelState["Photos"].ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
                {
                    ModelState.AddModelError("Photos", "Do not empty");
                }
                if (!slider.Photos.IsImage())
                {
                    ModelState.AddModelError("Photo", "Please upload only image files");
                    return View();
                }
                if (slider.Photos.IsCorrectSize(300))
                {
                    ModelState.AddModelError("Photo", "The photo size cannot be more than 300");
                    return View();
                }
                Slider dbslider = await _context.Sliders.FindAsync(id);
                string path = Path.Combine(_env.WebRootPath, "images", dbslider.ImageUrl);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                string filename = await slider.Photos.SaveImageAsync(_env.WebRootPath, "images");
                
                dbslider.ImageUrl = filename;
                dbslider.MainTitle = slider.MainTitle;
                dbslider.SubTitle = slider.SubTitle;
                dbslider.SliderText = slider.SliderText;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return NotFound();
            Slider dbSlider = await _context.Sliders.FindAsync(id);
            if (dbSlider == null) return NotFound();
            return View(dbSlider);
        }
    }
}
