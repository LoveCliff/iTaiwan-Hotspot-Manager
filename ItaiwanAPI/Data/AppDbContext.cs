using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ItaiwanAPI.Models;
namespace ItaiwanAPI.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        

        
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {

        }
        public DbSet<Hotspot> Hotspots { get; set; }  //Hotspots 表

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            // 對應到 "hotspots" (全小寫) 這張資料庫表
            modelBuilder.Entity<Hotspot>().ToTable("hotspots");
        }

    }


}
