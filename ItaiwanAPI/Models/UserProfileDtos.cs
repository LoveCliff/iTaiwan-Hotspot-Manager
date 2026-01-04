namespace ItaiwanAPI.Models 
{
    // 用於更新個人基本資料
    public class UpdateProfileDto
    {
        public required string Nickname { get; set; }
        public required string AvatarUrl { get; set; }
        public required string PhoneNumber { get; set; }
    }

    // 用於修改密碼
    public class ChangePasswordDto
    {
        public required string OldPassword { get; set; }
        public required string NewPassword { get; set; }
    }
}