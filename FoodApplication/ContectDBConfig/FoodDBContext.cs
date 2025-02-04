using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FoodApplication.Models;

namespace FoodApplication.ContectDBConfig
{
    public class FoodApplicationDBContext:IdentityDbContext<ApplicationUser>
    {
        public FoodApplicationDBContext(DbContextOptions<FoodApplicationDBContext> options):base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Cart> Carts { get; set; }
    }
}
