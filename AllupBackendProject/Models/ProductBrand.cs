using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AllupBackendProject.Models
{
    public class ProductBrand
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Can't be empty")]
        public string Name { get; set; }
        public List<BrandCategory> BrandCategories { get; set; }
        public virtual List<Product> Products { get; set; }
    }
}
