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
    public class AboutController : Controller
    {
        private readonly Context _context;

        public AboutController(Context context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<About> abouts = _context.About.ToList();
            About about = _context.About.FirstOrDefault();
            AboutVM aboutVM = new AboutVM();
            aboutVM.Abouts = abouts;
            aboutVM.About = about;
            ViewBag.About = aboutVM;
            return View(aboutVM);
        }

        
    }
}
