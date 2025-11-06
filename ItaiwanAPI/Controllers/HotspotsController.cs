using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ItaiwanAPI.Data;
using ItaiwanAPI.Models;


namespace ItaiwanAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HotspotsController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public HotspotsController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // 获取所有热点
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Hotspot>>> GetHotspots()
    {
        return await _dbContext.Hotspots.ToListAsync();
    }

    // 根据ID获取单个热点
    [HttpGet("{id}")]
    public async Task<ActionResult<Hotspot>> GetHotspot(int id)
    {
        var hotspot = await _dbContext.Hotspots.FindAsync(id);
        if (hotspot == null) return NotFound();
        return hotspot;
    }
}


