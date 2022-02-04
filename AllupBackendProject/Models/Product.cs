using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllupBackendProject.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ColorId { get; set; }
        public string ProductCode { get; set; }
        public bool Aviability { get; set; }
        public bool Featured { get; set; }
        public double Price { get; set; }
        public int BrandId { get; set; }
        public virtual Brand Brand { get; set; }
        public int Count { get; set; }
        //public List<SalesProduct> SalesProducts { get; set; }
    }
}
