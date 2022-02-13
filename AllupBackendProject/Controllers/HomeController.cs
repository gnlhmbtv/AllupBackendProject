using AllupBackendProject.DAL;
using AllupBackendProject.Models;
using AllupBackendProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace AllupBackendProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly Context _context;
        public HomeController(Context context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<Slider> sliders = _context.Sliders.ToList();
            SliderDescription sliderDescription = _context.SliderDescriptions.FirstOrDefault();
            List<Banner> banners = _context.Banners.ToList();
            List<Category> categories = _context.Categories.ToList();
            List<Product> products = _context.Products.Include(p => p.Campaign).Include(p => p.productPhotos).Include(p => p.Brand).ToList();
            Bio bio = _context.Bios.FirstOrDefault();
            HomeVm homeVm = new HomeVm();
            homeVm.Sliders = sliders;
            homeVm.SliderDescriptions = sliderDescription;
            homeVm.Banners = banners;
            homeVm.Categories = categories;
            homeVm.Bio = bio;
            homeVm.Products = products;
            ViewBag.newarrive = products.OrderByDescending(p => p.Id).Take(7).ToList();
            ViewBag.FeatCategories = categories.Where(c => c.IsFeatured == true);

            return View(homeVm);
        }
        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{ 
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}
