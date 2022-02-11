using AllupBackendProject.DAL;
using AllupBackendProject.Models;
using AllupBackendProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
            Bio bio = _context.Bios.FirstOrDefault();
            HomeVm homeVm = new HomeVm();
            homeVm.Sliders = sliders;
            homeVm.SliderDescriptions = sliderDescription;
            homeVm.Banners = banners;
            homeVm.Categories = categories;
            homeVm.Bio = bio;

           
            return View(homeVm);
        }
    }
}
