using ItaiwanAPI.Data;
using ItaiwanAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ItaiwanAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FavoritesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FavoritesController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/Favorites/5
        [HttpPost("{hotspotId}")]
        // 🔴 關鍵修改：這裡必須是 int hotspotId，不能是 string
        public async Task<IActionResult> AddFavorite(int hotspotId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            // 檢查是否已收藏
            var exists = await _context.UserFavorites
                .AnyAsync(f => f.UserId == userId && f.HotspotId == hotspotId);

            if (exists) return BadRequest("您已經收藏過此熱點");

            var favorite = new UserFavorite
            {
                UserId = userId,
                HotspotId = hotspotId // 這裡現在都是 int，不會報錯了
            };

            _context.UserFavorites.Add(favorite);
            await _context.SaveChangesAsync();

            return Ok(new { message = "收藏成功" });
        }

        // GET: api/Favorites
        [HttpGet]
        public async Task<IActionResult> GetMyFavorites()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var favorites = await _context.UserFavorites
                .Where(f => f.UserId == userId)
                .Include(f => f.Hotspot)
                .Select(f => f.Hotspot)
                .ToListAsync();

            return Ok(favorites);
        }

        // DELETE: api/Favorites/5
        [HttpDelete("{hotspotId}")]
        // 🔴 關鍵修改：這裡也必須是 int
        public async Task<IActionResult> RemoveFavorite(int hotspotId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var favorite = await _context.UserFavorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.HotspotId == hotspotId);

            if (favorite == null) return NotFound("未找到收藏記錄");

            _context.UserFavorites.Remove(favorite);
            await _context.SaveChangesAsync();

            return Ok(new { message = "已取消收藏" });
        }
    }
}