using Microsoft.EntityFrameworkCore;
using WebApi.Entities;

namespace WebApi
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            :base(options)
        {}

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Attribute> Attributes { get; set; }
        public DbSet<AttributeDetail> AttributeDetails { get; set; }
        public DbSet<ProductAttributeDetail> ProductAttributeDetails { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<UrlAddress> UrlAddresses { get; set; }
        public DbSet<PageUrlAddress> PageUrlAddresses { get; set; }
    }
}





