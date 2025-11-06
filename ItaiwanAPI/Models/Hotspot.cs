using ItaiwanAPI.Converts;
using System.Text.Json.Serialization;

namespace ItaiwanAPI.Models
{
    public class Hotspot
    {
        public int Id { get; set; } // 数据库自增主键
        public string Ministry { get; set; } = string.Empty; // 部会
        public string Area { get; set; } = string.Empty; // 区域
        public string Agency { get; set; } = string.Empty; // 单位
        public string Name { get; set; } = string.Empty; // 熱點名稱
        public string Address { get; set; } = string.Empty; // 地址
        public string Administration { get; set; } = string.Empty; // 管理单位

        [JsonConverter(typeof(StringToDoubleConverter))]
        public double Latitude { get; set; } // 纬度

        [JsonConverter(typeof(StringToDoubleConverter))]
        public double Longitude { get; set; } // 经度
    }
}
