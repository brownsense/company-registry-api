using marketplace.api.Gallery;
using marketplace.api.Products;
using marketplace.api.Recommendations;
using marketplace.api.RedFlags;
using marketplace.api.Users;
using Microsoft.EntityFrameworkCore;

namespace  MarketPlace.Api.Entities
{
    public class MarketplaceContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Entity> Entities { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Image> Images { get; set; }
        public virtual DbSet<RedFlag> RedFlags { get; set; }
        public virtual DbSet<Rating> Ratings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Marketplace;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
