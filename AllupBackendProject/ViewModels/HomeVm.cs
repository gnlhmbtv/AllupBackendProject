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
        public SliderDescription SliderDescriptions { get; set; }
    }
}
