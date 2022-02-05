using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AllupBackendProject.Models
{
    public class Blog
    {
        public int Id { get; set; }
        [Required]
        public string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        [MaxLength(50)]
        public string Title { get; set; }
        [MaxLength(80)]
        public string HomePageText { get; set; }
        [MaxLength(800)]
        public string BlogText { get; set; }
        [NotMapped]
        public IFormFile Photo { get; set; }
        //public AppUser CreatedByUser { get; set; }
    }
}
