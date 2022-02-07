using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AllupBackendProject.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public bool IsMain { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsDeleted { get; set; }
        public List<Category> SubCategory { get; set; }
        public Category MainCategory { get; set; }
        public List<BrandCategory> BrandCategories { get; set; }
        public string ImageUrl { get; set; }
        [NotMapped]
        public IFormFile Photo { get; set; }
    }
}
