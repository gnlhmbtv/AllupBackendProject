using AllupBackendProject.DAL;
using AllupBackendProject.Models;
using FrontToBack.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AllupBackendProject.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    [Authorize(Roles = "Admin")]

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
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            Product product = await _context.Products.FindAsync(id);
            var relation = _context.ProductRelations.Where(b => b.ProductId == id && b.BrandId == product.BrandId).FirstOrDefault();
            Category category = await _context.Categories.FindAsync(relation.CategoryId);
            var brands = new SelectList(_context.ProductBrands.OrderBy(l => l.Name)
            .ToDictionary(us => us.Id, us => us.Name), "Key", "Value");
            ViewBag.BrandId = brands;
            var campaign = new SelectList(_context.Campaigns.OrderBy(l => l.Discount)
             .ToDictionary(us => us.Id, us => us.Discount), "Key", "Value");
            ViewBag.CampaignId = campaign;
            ViewBag.category = category;
            var photos = _context.ProductPhotos.Where(p => p.ProductId == id).ToList();
            ViewBag.photos = photos;

            var checkTag = await _context.ProductTags.Where(p => p.ProductId == id).Select(t => t.Tag).ToListAsync();
            var checkColor = await _context.ColorProducts.Where(p => p.ProductId == id).Select(c => c.Color).ToListAsync();
            ViewBag.checkTag = checkTag;
            ViewBag.checkColor = checkColor;

            var allTag = await _context.Tags.ToListAsync();
            var allColor = await _context.Colors.ToListAsync();

            var noneCheckTag = allTag.Except(checkTag);
            var noneCheckColor = allColor.Except(checkColor);
            ViewBag.noneTag = noneCheckTag;
            ViewBag.noneColor = noneCheckColor;

            return View(product);
        }

        // POST: ProductController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int? id, Product product, int categoryid, List<int> tagId, List<int> colorId)
        {

            Product newProduct = await _context.Products.FindAsync(id);
            var relationProduct = _context.ProductRelations.Where(p => p.ProductId == id && p.BrandId == newProduct.BrandId).FirstOrDefault();

            newProduct.Name = product.Name;
            newProduct.Price = product.Price;
            newProduct.CampaignId = product.CampaignId;
            newProduct.BrandId = product.BrandId;
            newProduct.Availibility = product.Availibility;
            newProduct.Description = product.Description;
            newProduct.ExTax = product.ExTax;
            newProduct.Quantity = product.Quantity;
            newProduct.Featured = product.Featured;
            newProduct.ProductCode = product.ProductCode;
            await _context.SaveChangesAsync();

            if (relationProduct.CategoryId != categoryid || relationProduct.BrandId != newProduct.BrandId)
            {
                _context.ProductRelations.Remove(relationProduct);
                ProductRelation newProductRelation = new ProductRelation();
                newProductRelation.CategoryId = categoryid;
                newProductRelation.BrandId = newProduct.BrandId;
                newProductRelation.ProductId = newProduct.Id;
                await _context.ProductRelations.AddAsync(newProductRelation);
                await _context.SaveChangesAsync();
            }

            List<int> checkTag = _context.ProductTags.Where(p => p.ProductId == newProduct.Id).Select(t => t.TagId).ToList();
            List<int> checkColor = _context.ColorProducts.Where(c => c.ProductId == newProduct.Id).Select(c => c.ColorId).ToList();

            List<int> addedTag = tagId.Except(checkTag).ToList();
            List<int> removeTag = checkTag.Except(tagId).ToList();

            List<int> addedColor = colorId.Except(checkColor).ToList();
            List<int> removeColor = checkColor.Except(colorId).ToList();
            int addedTagLength = addedTag.Count();
            int removedTagLength = removeTag.Count();
            int FullLength = addedTagLength + removedTagLength;

            int addedColorLength = addedColor.Count();
            int removedColorLength = removeColor.Count();
            int FullLengthColor = addedColorLength + removedColorLength;



            for (int i = 1; i <= FullLength; i++)
            {
                if (addedTagLength >= i)
                {
                    ProductTag productTag = new ProductTag();
                    productTag.ProductId = newProduct.Id;
                    productTag.TagId = addedTag[i - 1];
                    await _context.ProductTags.AddAsync(productTag);
                    await _context.SaveChangesAsync();
                }

                if (removedTagLength >= i)
                {
                    ProductTag productTag = await _context.ProductTags.FirstOrDefaultAsync(c => c.TagId == removeTag[i - 1] && c.ProductId == newProduct.Id);
                    _context.ProductTags.Remove(productTag);
                    await _context.SaveChangesAsync();
                }
            }

            for (int i = 1; i <= FullLengthColor; i++)
            {
                if (addedTagLength >= i)
                {
                    ColorProduct colorProduct = new ColorProduct();
                    colorProduct.ProductId = newProduct.Id;
                    colorProduct.ColorId = addedColor[i - 1];
                    await _context.ColorProducts.AddAsync(colorProduct);
                    await _context.SaveChangesAsync();
                }

                if (removedTagLength >= i)
                {
                    ColorProduct colorProduct = await _context.ColorProducts.FirstOrDefaultAsync(c => c.ColorId == removeColor[i - 1] && c.ProductId == newProduct.Id);
                    _context.ColorProducts.Remove(colorProduct);
                    await _context.SaveChangesAsync();
                }
            }

            if (product.Photos != null)
            {
                if (ModelState["Photos"].ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
                {
                    ModelState.AddModelError("Photo", "Do not empty");
                }
                var count = 0;
                var oldPhoto = _context.ProductPhotos.Where(p => p.ProductId == newProduct.Id).ToList();


                if (oldPhoto.Count <= product.Photos.Length)
                {
                    foreach (var item in oldPhoto)
                    {
                        string path = Path.Combine(_env.WebRootPath, "images/product/", item.PhotoUrl);
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                        _context.ProductPhotos.Remove(item);
                        await _context.SaveChangesAsync();
                    }
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
                        productPhoto.ProductId = newProduct.Id;

                        await _context.ProductPhotos.AddAsync(productPhoto);
                        await _context.SaveChangesAsync();
                        count++;
                    }
                }
                else
                {
                    for (int i = 0; i < product.Photos.Length; i++)
                    {
                        if (!product.Photos[i].IsImage())
                        {
                            ModelState.AddModelError("Photo", "only image");
                            return RedirectToAction("Index");

                        }
                        if (product.Photos[i].IsCorrectSize(300))
                        {
                            ModelState.AddModelError("Photo", "please enter photo under 300kb");
                            return RedirectToAction("Index");
                        }
                        string fileName = await product.Photos[i].SaveImageAsync(_env.WebRootPath, "images/product/");
                        string path = Path.Combine(_env.WebRootPath, "images/product/", oldPhoto[i].PhotoUrl);
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                        oldPhoto[i].PhotoUrl = fileName;
                        await _context.SaveChangesAsync();

                    }
                }

            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: ProductController/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            Product product = await _context.Products.Include(p => p.Campaign).Include(p => p.Brand).FirstOrDefaultAsync(p => p.Id == id);
            var relation = _context.ProductRelations.Where(b => b.ProductId == id && b.BrandId == product.BrandId).FirstOrDefault();
            Category category = await _context.Categories.FindAsync(relation.CategoryId);
            ViewBag.category = category;

            ViewBag.color = await _context.ColorProducts.Where(p => p.ProductId == id).Select(c => c.Color).ToListAsync();
            ViewBag.photo = _context.ProductPhotos.Where(p => p.ProductId == product.Id && p.IsMain == true).FirstOrDefault();
            return View(product);
        }

        // POST: ProductController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int? id, Product product)
        {
            var newProduct = await _context.Products.FindAsync(id);
            if (newProduct == null) return NotFound();

            var productColor = _context.ColorProducts.Where(p => p.ProductId == id).ToList();
            if (productColor != null)
            {
                foreach (var item in productColor)
                {
                    _context.ColorProducts.Remove(item);
                    await _context.SaveChangesAsync();
                }
            }
            var productTags = _context.ProductTags.Where(p => p.ProductId == id).ToList();
            if (productTags != null)
            {
                foreach (var item in productTags)
                {
                    _context.ProductTags.Remove(item);
                    await _context.SaveChangesAsync();
                }
            }
            var relationProduct = _context.ProductRelations.FirstOrDefault(p => p.ProductId == id && p.BrandId == newProduct.BrandId);
            _context.ProductRelations.Remove(relationProduct);


            if (newProduct.Photos != null)
            {
                var oldPhoto = _context.ProductPhotos.Where(p => p.ProductId == newProduct.Id).ToList();
                foreach (var item in oldPhoto)
                {
                    string path = Path.Combine(_env.WebRootPath, "images/product/", item.PhotoUrl);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                    _context.ProductPhotos.Remove(item);
                    await _context.SaveChangesAsync();
                }
            }
            _context.Products.Remove(newProduct);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
