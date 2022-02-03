using AllupBackendProject.DAL;
using AllupBackendProject.Models;
using AllupBackendProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
            HomeVm homeVm = new HomeVm();
            homeVm.Sliders = sliders;
            homeVm.SliderDescriptions = sliderDescription;
            return View(homeVm);
        }
    }
}
