using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AllupBackendProject.Models
{
    public class Feature
    {
        public int Id { get; set; }
        public string BannerTitle { get; set; }
        public string BannerText { get; set; }
        public string ImageUrl { get; set; }
        [NotMapped] 
        [Required]
        public IFormFile Photo { get; set; }
    }
}
