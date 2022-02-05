using AllupBackendProject.DAL;
using AllupBackendProject.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllupBackendProject.ViewComponents
{
    public class BrandsViewComponent : ViewComponent
    {
        private readonly Context _context;
        public BrandsViewComponent(Context context)
        {
            _context = context;
        }
         public async Task<IViewComponentResult> InvokeAsync()
        {
            List<Brand> brand = _context.Brands.ToList();
            return View(await Task.FromResult(brand));
        }
    }
}
