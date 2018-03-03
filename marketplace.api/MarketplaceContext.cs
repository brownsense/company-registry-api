using marketplace.api.Users;
using Microsoft.EntityFrameworkCore;

namespace  MarketPlace.Api.Entities
{
    public class MarketplaceContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Entity> Entities { get; set; }
    }
}
