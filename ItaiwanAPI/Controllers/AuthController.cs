using ItaiwanAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ItaiwanAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        // 🟢 註冊接口：使用 RegisterDto
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            // 檢查 Username 或 Email 是否已被註冊 (雖然 Identity 會檢查，但手動檢查回傳訊息更明確)
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null) return BadRequest(new { message = "使用者名稱已被使用" });

            var emailExists = await _userManager.FindByEmailAsync(model.Email);
            if (emailExists != null) return BadRequest(new { message = "Email 已被註冊" });

            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                Nickname = model.Username // 預設暱稱
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok(new { message = "註冊成功" });
            }
            return BadRequest(result.Errors);
        }

        // 🔵 登錄接口：使用 LoginDto
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            ApplicationUser? user = null;

            // 🚀 性能優化邏輯：
            // 前端只傳一個 "Account" 字串進來。
            // 我們判斷：如果包含 '@' 符號，就先去查 Email；否則查 Username。
            // 這樣可以避免每次都查兩次資料庫。

            if (model.Account.Contains("@"))
            {
                user = await _userManager.FindByEmailAsync(model.Account);
            }

            // 如果用 Email 沒找到，或者輸入的不含 @ (代表是用戶名)，則嘗試用 Name 查找
            if (user == null)
            {
                user = await _userManager.FindByNameAsync(model.Account);
            }

            // 驗證密碼
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                // 登錄成功，生成 JWT Token
                var token = GenerateJwtToken(user);
                return Ok(new { token, message = "登入成功" });
            }

            // 統一錯誤訊息，避免洩漏具體是帳號錯還是密碼錯 (安全性考量)
            return BadRequest(new { message = "帳號或密碼錯誤" });
        }

        // ... GenerateJwtToken 方法保持不變 ...
        private string GenerateJwtToken(ApplicationUser user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var keyString = _configuration["Jwt:Key"] ?? "ThisIsASecretKey1234567890PleaseChangeItInProduction";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "http://localhost:5143",
                audience: "http://localhost:5143",
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
    // 1. 註冊專用 DTO (欄位分開，確保資料完整)
    public class RegisterDto
    {
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    // 2. 登入專用 DTO (合併欄位，單一入口)
    public class LoginDto
    {
        // 這裡的 Account 可以接收 Username 或 Email
        public required string Account { get; set; }
        public required string Password { get; set; }
    }

}
