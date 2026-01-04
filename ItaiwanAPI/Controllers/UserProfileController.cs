using ItaiwanAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
[Authorize] // 🔒 只有登入用戶才能訪問
public class UserProfileController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserProfileController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    // 1. 獲取當前用戶資料 (回填表單用)
    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound("找不到用戶");

        return Ok(new
        {
            user.Nickname,
            user.AvatarUrl,
            user.Email,
            user.PhoneNumber
        });
    }

    // 2. 修改基本資料
    [HttpPut("update-info")]
    public async Task<IActionResult> UpdateInfo([FromBody] UpdateProfileDto dto)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        // 更新欄位
        user.Nickname = dto.Nickname;
        user.AvatarUrl = dto.AvatarUrl;
        user.PhoneNumber = dto.PhoneNumber;

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
            return Ok(new { message = "資料更新成功" });

        return BadRequest(result.Errors);
    }

    // 3. 修改密碼
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        // Identity 內建方法：驗證舊密碼並設定新密碼
        var result = await _userManager.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);

        if (result.Succeeded)
            return Ok(new { message = "密碼修改成功" });

        return BadRequest(new { message = "密碼修改失敗，請檢查舊密碼是否正確" });
    }
}