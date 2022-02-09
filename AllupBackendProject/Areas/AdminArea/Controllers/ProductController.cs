using AllupBackendProject.DAL;
using AllupBackendProject.Models;
using FrontToBack.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllupBackendProject.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    public class ProductController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly Context _context;

        public ProductController(Context context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        // GET: ProductController
        public ActionResult Index()
        {
            List<Product> product = _context.Products.Include(c => c.Campaign).Include(b => b.Brand).ToList();
            return View(product);
        }

        // GET: ProductController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        public ActionResult CallCategory(int? id)
        {
            List<Category> categories = _context.BrandCategories.Where(b => b.ProductBrandId == id)
                .Select(c => c.Category).ToList();

            return PartialView("_ProductCreatePartial", categories);
        }

        // GET: ProductController/Create
        public ActionResult Create()
        {
            var brands = new SelectList(_context.ProductBrands.OrderBy(l => l.Name)
            .ToDictionary(us => us.Id, us => us.Name), "Key", "Value");
            ViewBag.Brand = brands;
            var campaign = new SelectList(_context.Campaigns.OrderBy(l => l.Discount)
             .ToDictionary(us => us.Id, us => us.Discount), "Key", "Value");
            ViewBag.Campaign = campaign;
            var colors = _context.Colors.ToList();
            var tags = _context.Tags.ToList();
            ViewBag.Tags = tags;
            ViewBag.Colors = colors;
            return View();
        }

        // POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Product product, int categoryid, int[] tagId, int[] colorId)
        {
            bool isExistProduct = _context.Products.Any(c => c.Name.ToLower() == product.Name.ToLower().Trim());

            if (isExistProduct)
            {
                ModelState.AddModelError("Name", "This product is already exists");
                return RedirectToAction("Index");
            }
            if (categoryid == 0) return NotFound();

            Product newproduct = new Product()
            {
                Name = product.Name,
                Price = product.Price,
                CampaignId = product.CampaignId,
                BrandId = product.BrandId,
                Availibility = product.Availibility,
                Description = product.Description,
                ExTax = product.ExTax,
                Quantity = product.Quantity,
                Featured = product.Featured,
                ProductCode = product.ProductCode
            };


            await _context.Products.AddAsync(newproduct);
            await _context.SaveChangesAsync();
            ProductRelation productRelation = new ProductRelation()
            {
                ProductId = newproduct.Id,
                BrandId = newproduct.BrandId,
                CategoryId = categoryid
            };
            await _context.ProductRelations.AddAsync(productRelation);
            if (tagId != null && colorId != null)
            {
                foreach (var item in tagId)
                {
                    ProductTag productTag = new ProductTag()
                    {
                        ProductId = newproduct.Id,
                        TagId = item,
                    };
                    await _context.ProductTags.AddAsync(productTag);
                    await _context.SaveChangesAsync();
                }
                foreach (var item in colorId)
                {
                    ColorProduct colorProduct = new ColorProduct()
                    {
                        ProductId = newproduct.Id,
                        ColorId = item,
                    };
                    await _context.ColorProducts.AddAsync(colorProduct);
                    await _context.SaveChangesAsync();
                }
            }
            if (ModelState["Photos"].ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
            {
                ModelState.AddModelError("Photo", "Do not empty");
            }
            var count = 0;
            foreach (IFormFile photo in product.Photos)
            {
                if (!photo.IsImage())
                {
                    ModelState.AddModelError("Photo", "only image");
                    return RedirectToAction("Index");

                }
                if (photo.IsCorrectSize(300))
                {
                    ModelState.AddModelError("Photo", "please enter photo under 300kb");
                    return RedirectToAction("Index");
                }
                ProductPhoto productPhoto = new ProductPhoto();

                string fileName = await photo.SaveImageAsync(_env.WebRootPath, "images/product/");
                if (count == 0) productPhoto.IsMain = true;
                productPhoto.PhotoUrl = fileName;
                productPhoto.ProductId = newproduct.Id;
                await _context.ProductPhotos.AddAsync(productPhoto);
                await _context.SaveChangesAsync();
                count++;
            }
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        // GET: ProductController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ProductController/Edit/5
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

        // GET: ProductController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ProductController/Delete/5
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
