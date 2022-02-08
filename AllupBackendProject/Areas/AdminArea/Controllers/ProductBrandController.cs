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
            ProductBrand brand = await _context.ProductBrands.Include(b => b.BrandCategories).ThenInclude(c => c.Category).FirstOrDefaultAsync(c => c.Id == id);
            List<BrandCategory> SubCategory = await _context.BrandCategories.Include(c => c.Category).Where(x => x.ProductBrandId == brand.Id).ToListAsync();

            List<Category> AllCategory = await _context.Categories.Include(c => c.BrandCategories).ThenInclude(c => c.ProductBrand).Where(c => c.IsMain == false).ToListAsync();
            foreach (var item in SubCategory)
            {
                AllCategory.Remove(item.Category);

            }
            ViewBag.checkCategory = SubCategory;
            ViewBag.noneCheck = AllCategory;
            return View(brand);
        }

        // POST: ProductBrandController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, ProductBrand brand, int[] subcategory)
        {
            bool isExist = _context.Brands.Any(c => c.Name.ToLower() == brand.Name.ToLower().Trim());
            ProductBrand newBrand = await _context.ProductBrands.FindAsync(id);

            if (isExist && !(newBrand.Name.ToLower() == brand.Name.ToLower().Trim()))
            {
                ModelState.AddModelError("Name", "There is already has a brand with this name.");
                return RedirectToAction("Index");
            }
            var allSubCategory = _context.Categories.Where(c => c.IsMain == false).ToList();
            var checkedCategory = _context.BrandCategories.Where(c => c.ProductBrandId == newBrand.Id).ToList();

            List<int> allCategoryList = new List<int>();
            List<int> checkedCategoryList = new List<int>();

            foreach (var item in allSubCategory)
            {
                allCategoryList.Add(item.Id);
            }

            foreach (var item in checkedCategory)
            {
                checkedCategoryList.Add(item.CategoryId);
            }

            var addedCategory = subcategory.Except(checkedCategoryList);
            var removedCategory = checkedCategoryList.Except(subcategory);

            newBrand.Name = brand.Name;


            foreach (var item in removedCategory)
            {
                BrandCategory categoryBrand = await _context.BrandCategories.Where(c => c.CategoryId == item && c.ProductBrandId == newBrand.Id).FirstOrDefaultAsync();
                _context.BrandCategories.Remove(categoryBrand);
                await _context.SaveChangesAsync();
            }
            if (addedCategory != null)
            {
                foreach (var item in addedCategory)
                {
                    Category category = _context.Categories.Find(item);
                    BrandCategory categoryBrand = new BrandCategory();
                    categoryBrand.ProductBrandId = newBrand.Id;
                    categoryBrand.CategoryId = category.Id;
                    await _context.BrandCategories.AddAsync(categoryBrand);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction("Index");
            }
            return NotFound();



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
