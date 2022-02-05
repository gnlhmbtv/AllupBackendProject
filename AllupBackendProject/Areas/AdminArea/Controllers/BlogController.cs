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
    public class BlogController : Controller
    {
        private readonly Context _context;
        private readonly IWebHostEnvironment _env;
        public BlogController(Context context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            List<Blog> blogs = _context.Blogs.ToList();
            return View(blogs);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Create(Blog blog)
        {
            if (ModelState["Photo"].ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
            {
                ModelState.AddModelError("Photos", "Do not empty");
            }
            if (!blog.Photo.IsImage())
            {
                ModelState.AddModelError("Photo", "Please upload only image files");
                return View();
            }
            if (blog.Photo.IsCorrectSize(300))
            {
                ModelState.AddModelError("Photo", "The photo size cannot be more than 300");
                return View();
            }
            Blog newBlog = new Blog();

            string fileName = await blog.Photo.SaveImageAsync(_env.WebRootPath, "images");
            newBlog.ImageUrl = fileName;
            newBlog.Title = blog.Title;
            newBlog.BlogText = blog.BlogText;
            newBlog.HomePageText = blog.HomePageText;
            newBlog.CreatedAt = DateTime.Now;
            await _context.Blogs.AddAsync(newBlog);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            Blog dbBlog = await _context.Blogs.FindAsync(id);
            if (dbBlog == null) return NotFound();
            return View(dbBlog);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteBlog(int? id)
        {
            if (id == null) return NotFound();
            Blog dbBlog = await _context.Blogs.FindAsync(id);
            if (dbBlog == null) return NotFound();

            string path = Path.Combine(_env.WebRootPath, "image", dbBlog.ImageUrl);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            _context.Blogs.Remove(dbBlog);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return NotFound();
            Blog dbBlog = await _context.Blogs.FindAsync(id);
            if (dbBlog == null) return NotFound();
            return View(dbBlog);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Update(int? id, Blog blog)
        {
            Blog dbBlog = await _context.Blogs.FindAsync(id);

            if (blog.Photo != null)
            {
                if (ModelState["Photo"].ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
                {
                    ModelState.AddModelError("Photo", "Do not empty");
                }
                if (!blog.Photo.IsImage())
                {
                    ModelState.AddModelError("Photo", "Please upload only image files");
                    return View();
                }
                if (blog.Photo.IsCorrectSize(300))
                {
                    ModelState.AddModelError("Photo", "The photo size cannot be more than 300");
                    return View();
                }
                string path = Path.Combine(_env.WebRootPath, "images", dbBlog.ImageUrl);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                string filename = await blog.Photo.SaveImageAsync(_env.WebRootPath, "images");
                dbBlog.ImageUrl = filename;

            }
            dbBlog.Title = blog.Title;
            dbBlog.BlogText = blog.BlogText;
            dbBlog.HomePageText = blog.HomePageText;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return NotFound();
            Blog dbBlog = await _context.Blogs.FindAsync(id);
            if (dbBlog == null) return NotFound();
            return View(dbBlog);
        }
    }
}
