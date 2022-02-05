﻿using AllupBackendProject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllupBackendProject.DAL
{
    public class Context:DbContext
    {
        public Context(DbContextOptions<Context>options):base(options)
        {

        }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<SliderDescription> SliderDescriptions { get; set; }
        public DbSet<Banner> Banners { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Bio> Bios { get; set; }
        public DbSet<BrandBanner> Brands { get; set; }
        public DbSet<Mail> Mails { get; set; }




        public DbSet<Feature> Features { get; set; }

    }
}
