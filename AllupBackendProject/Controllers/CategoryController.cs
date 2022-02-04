using AllupBackendProject.DAL;
using AllupBackendProject.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllupBackendProject.Controllers
{
    public class CategoryController
    {
        private readonly Context _context;
        public CategoryController(Context context)
        {
            _context = context;
        }

        //public IActionResult Index()
        //{
        //    IEnumerable<Category> categories = _context.Categories.ToList();

        //    return RedirectToActionResy("Index","Home");
        //}
    }
}
