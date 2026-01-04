using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ItaiwanAPI.Models 
{
    public class UserFavorite
    {
        [Key]
        public int Id { get; set; }

        // 記錄是用哪個用戶收藏的
        [Required]
        public string UserId { get; set; }

        // 記錄收藏了哪個熱點
        public int HotspotId { get; set; }

        // 建立導航屬性 (方便我們直接查詢熱點的詳細信息)
        [ForeignKey("HotspotId")]
        public Hotspot? Hotspot { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}

