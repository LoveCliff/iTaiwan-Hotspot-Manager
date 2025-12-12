using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ItaiwanAPI.Data;
using ItaiwanAPI.Models;
using System;
using System.Linq;
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

    // 獲取所有網絡熱點
    [HttpGet]
    public async Task<ActionResult<PagedResult<object>>> GetHotspots(
    string? keyword,
    double? lat,
    double? lon,
    int page = 1,
    int pageSize = 10)
    {
        // 基础查询 (此時還未查詢資料庫)

        var query = _dbContext.Hotspots.AsQueryable();

        //  篩選關鍵字
        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(h => h.Name.Contains(keyword) || h.Address.Contains(keyword));
        }

        
        List<object> resultList;
        int totalCount = 0;

        // 3. 分支逻辑：是否需要地理位置排序？
        if (lat.HasValue && lon.HasValue)
        {
            // === 模式 A: 經緯度排序模式 ===
          
            var allMatches = await query.ToListAsync(); 

            // 記憶體計算距離
            var sortedData = allMatches.Select(h => new
            {
                h.Id,
                h.Name,
                h.Address,
                h.Latitude,
                h.Longitude,
                DistanceKm = Math.Round(CalculateDistance(lat.Value, lon.Value, h.Latitude, h.Longitude), 2)
            })
            .OrderBy(x => x.DistanceKm); // 按距離排序

            totalCount = sortedData.Count();

            // 記憶體分頁
            resultList = sortedData
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Cast<object>() 
                .ToList();
        }
        else
        {
            // 模式 B: 普通列表模式
            // 直接在資料庫層分頁

            totalCount = await query.CountAsync(); // 先查总数

            var dbList = await query
                .OrderBy(h => h.Id) // 默认按ID排序，防止分页乱序
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(h => new
                {
                    h.Id,
                    h.Name,
                    h.Address,
                    h.Latitude,
                    h.Longitude,
                    DistanceKm = (double?)null // 没有距离
                })
                .ToListAsync();

            resultList = dbList.Cast<object>().ToList();
        }

        // 4. 组装返回结果
        var result = new PagedResult<object>
        {
            Items = resultList,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };

        return Ok(result);
    }

    //Haversine 公式計算兩點之間的距離
    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        var R = 6371; // 地球半径 (km)
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private double ToRadians(double angle) => Math.PI * angle / 180.0;

    // 根據ID獲取單個網絡熱點
    [HttpGet("{id}")]
    public async Task<ActionResult<Hotspot>> GetHotspot(int id)
    {
        var hotspot = await _dbContext.Hotspots.FindAsync(id);
        if (hotspot == null) return NotFound();
        return hotspot;
    }
}


