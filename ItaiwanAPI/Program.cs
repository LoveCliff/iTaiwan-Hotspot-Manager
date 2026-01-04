using ItaiwanAPI.Data;
using ItaiwanAPI.Models;
using ItaiwanAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IO;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// 注册MySQL上下文
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("MySqlConnection"),
        new MySqlServerVersion(new Version(8, 0, 42)) 
    )
);

// 注册 DataImportService
builder.Services.AddScoped<DataImportService>();



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // 前端Vite默认端口
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 添加控制器服務
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "ItaiwanAPI", Version = "v1" });
});


//配置identity和身份驗證（ApplicationUser是繼承後的子類，添加了nickname和頭像）
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// JWT驗證
var jwtKey = "ThisIsASecretKey1234567890PleaseChangeItInProduction"; // 密钥(生产环境要很长且保密)
var jwtIssuer = "http://localhost:5143";
var jwtAudience = "http://localhost:5143";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});


var app = builder.Build();
//app.UseSwagger();
//app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ItaiwanAPI v1"));

//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    var importService = services.GetRequiredService<DataImportService>();
//    var jsonPath = Path.Combine(AppContext.BaseDirectory, "App_Data", "IpSelect_tw.json");
//    await importService.ImportFromJsonAsync(jsonPath);
//}



// 启用控制器路由
app.UseCors("AllowVueApp");
app.UseAuthentication(); // 識別你是誰
app.UseAuthorization();  // 確認你有沒有權限
app.MapControllers();
app.Run();



