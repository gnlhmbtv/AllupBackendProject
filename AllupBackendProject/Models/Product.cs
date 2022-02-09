using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AllupBackendProject.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public double ExTax { get; set; }
        [Required]
        public string ProductCode { get; set; }
        public bool Availibility { get; set; }
        [Required]
        public int Quantity { get; set; }
        public bool Featured { get; set; }
        public int BrandId { get; set; }
        public ProductBrand Brand { get; set; }
        public int CampaignId { get; set; }
        public Campaign Campaign { get; set; }
        public List<ProductPhoto> productPhotos { get; set; }
        [NotMapped]
        [Required]
        public IFormFile[] Photos { get; set; }
        //public List<SalesProduct> SalesProducts { get; set; }
    }
}
