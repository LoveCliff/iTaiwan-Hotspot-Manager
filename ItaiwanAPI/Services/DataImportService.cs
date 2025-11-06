using ItaiwanAPI.Converts;
using ItaiwanAPI.Data;
using ItaiwanAPI.Models;
using System.IO;
using System.Text.Json;
namespace ItaiwanAPI.Services
{
    public class DataImportService
    {
        private readonly AppDbContext _dbContext;

        public DataImportService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // 从JSON文件导入数据到MySQL

        public async Task ImportFromJsonAsync(string jsonPath)
        {
            if (!File.Exists(jsonPath))
                throw new FileNotFoundException("JSON文件不存在", jsonPath);

            var jsonContent = await File.ReadAllTextAsync(jsonPath);
            var options = new JsonSerializerOptions
            {
                Converters = { new StringToDoubleConverter() },
                PropertyNameCaseInsensitive = true // 忽略JSON字段大小写（可选，若字段名不一致）
            };
            var hotspots = JsonSerializer.Deserialize<List<Hotspot>>(jsonContent, options);

            if (hotspots != null && hotspots.Any())
            {
                // 清空旧数据（可选）
                _dbContext.Hotspots.RemoveRange(_dbContext.Hotspots);
                await _dbContext.SaveChangesAsync();

                // 插入新数据
                _dbContext.Hotspots.AddRange(hotspots);
                await _dbContext.SaveChangesAsync();
                Console.WriteLine($"成功导入 {hotspots.Count} 个iTaiwan热点数据！");
            }
        }
    }
}
