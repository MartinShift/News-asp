
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using News.Models;

namespace News.DbModels
{
    public class NewsDbContext : IdentityDbContext<User,IdentityRole<int>,int>
    {
        public NewsDbContext(DbContextOptions<NewsDbContext> options) : base(options)
        {
        }
        public NewsDbContext() : base() { }
        public DbSet<DbNewsModel> News { get; set; }
        public DbSet<DbImageFile> Images { get; set; }
        public DbSet<Waypoint> Waypoints { get; set; }
        public DbSet<Todo> Todos { get; set; }
    }

}
