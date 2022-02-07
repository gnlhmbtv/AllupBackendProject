using AllupBackendProject.DAL;
using AllupBackendProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllupBackendProject.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    public class ProductBrandController : Controller
    {
        private readonly Context _context;

        public ProductBrandController(Context context)
        {
            _context = context;
        }
        // GET: ProductBrandController
        public ActionResult Index()
        {
            List<ProductBrand> brands = _context.ProductBrands.Include(b => b.BrandCategories).ThenInclude(c => c.Category).ToList();
            return View(brands);
        }

        // GET: ProductBrandController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ProductBrandController/Create
        public ActionResult Create()
        {
            ViewBag.IsMainCategory = _context.Categories.Where(c => c.IsMain == true).ToList();
            ViewBag.SubCategory = _context.Categories.Where(c => c.IsMain == false).ToList();

            return View();
        }

        // POST: ProductBrandController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ProductBrand brand, int[] subcategory)
        {
            bool isExist = _context.Brands.Any(c => c.Name.ToLower() == brand.Name.ToLower().Trim());

            if (isExist)
            {
                ModelState.AddModelError("Name", "This category is already exists");
                return RedirectToAction("Index");

            }


            ProductBrand newBrand = new ProductBrand();
            newBrand.Name = brand.Name;
            await _context.ProductBrands.AddAsync(newBrand);
            await _context.SaveChangesAsync();


            if (subcategory != null)
            {

                foreach (var item in subcategory)
                {
                    BrandCategory categoryBrands = new BrandCategory();
                    categoryBrands.ProductBrandId = newBrand.Id;
                    categoryBrands.CategoryId = item;
                    await _context.AddAsync(categoryBrands);
                    await _context.SaveChangesAsync();
                }

            }
            return RedirectToAction(nameof(Index));
        }

        // GET: ProductBrandController/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            ProductBrand dbBrand = await _context.ProductBrands.FindAsync(id);
            if (dbBrand == null) return NotFound();
            return View(dbBrand);
        }

        // POST: ProductBrandController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ProductBrandController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ProductBrandController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
