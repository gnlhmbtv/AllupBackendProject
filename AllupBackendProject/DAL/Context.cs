﻿using AllupBackendProject.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllupBackendProject.DAL
{
    public class Context:IdentityDbContext<AppUser>
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
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<About> About { get; set; }
        public DbSet<ProductBrand> ProductBrands { get; set; }
        public DbSet<BrandCategory> BrandCategories { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<ProductPhoto> ProductPhotos { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<ColorProduct> ColorProducts { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ProductTag> ProductTags { get; set; }
        public DbSet<ProductRelation> ProductRelations { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<BlogPhoto> BlogPhoto { get; set; }
        public DbSet<Sales> Sales { get; set; }
        public DbSet<SalesProduct> SalesProducts { get; set; }
    }
}
