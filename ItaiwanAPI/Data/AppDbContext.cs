using Microsoft.EntityFrameworkCore;
using ItaiwanAPI.Models;
namespace ItaiwanAPI.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Hotspot> Hotspots { get; set; } // 对应数据库的 Hotspots 表

        // 必须的构造函数，以支持 builder.Services.AddDbContext<AppDbContext>(...)
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {

        }
    }
}
