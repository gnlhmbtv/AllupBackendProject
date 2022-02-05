using AllupBackendProject.DAL;
using AllupBackendProject.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllupBackendProject.ViewComponents
{
    public class FeaturesViewComponent : ViewComponent
    {
        private readonly Context _context;
        public FeaturesViewComponent(Context context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<Feature> features = _context.Features.ToList();
            return View(await Task.FromResult(features));
        }
    }
}
