using AllupBackendProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllupBackendProject.ViewModels
{
    public class HomeVm
    {
        public List<Slider> Sliders { get; set; }
        public Slider Slider { get; set; }
        public SliderDescription SliderDescriptions { get; set; }
        public List<Banner> Banners { get; set; }
        public List<Category> Categories { get; set; }
        public Bio Bio { get; set; }
        public Contact Contact { get; set; }
        public List<Product> Products { get; set; }
        public List<Blog> Blogs { get; set; }

    }
}
