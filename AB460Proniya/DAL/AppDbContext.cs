using AB460Proniya.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using Color = AB460Proniya.Models.Color;
using Size = AB460Proniya.Models.Size;

namespace AB460Proniya.DAL
{

    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {

        }


        public DbSet<ProductTag> ProductTags { get; set; }

        public DbSet<Tag> Tags { get; set; }
        public DbSet<Product> Products { get; set; }

        public DbSet<Slide> Slides { get; set; }

        public DbSet<Category> Categories { get; set; }


        public DbSet<ProductImage> ProductImages { get; set; }


        public DbSet<ProductSize> ProductSizes { get; set; }

        public DbSet<Size> Sizes { get; set; }

        public DbSet<Color> Colors { get; set; }
        public DbSet<ProductColor> ProductColors { get; set; }

        public DbSet<Setting> Settings { get; set; }

      
    }





}
