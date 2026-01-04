using Microsoft.AspNetCore.Identity;

namespace ItaiwanAPI.Models
{
    // 繼承 IdentityUser，這樣我們就擁有了所有預設功能，再加上我們自定義的欄位
    public class ApplicationUser : IdentityUser
    {
        public string? Nickname { get; set; }
        public string? AvatarUrl { get; set; }
    }
}