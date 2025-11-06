using ItaiwanAPI.Data;
using ItaiwanAPI.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// 注册MySQL上下文
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("MySqlConnection"),
        new MySqlServerVersion(new Version(8, 0, 42)) 
    )
);

// 注册 DataImportService（必须与 AppDbContext 同生命周期，用 AddScoped）
builder.Services.AddScoped<DataImportService>();



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp", policy =>
    {
        policy.WithOrigins("http://localhost:5174") // 前端Vite默认端口
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 添加控制器支持
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "ItaiwanAPI", Version = "v1" });
});

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ItaiwanAPI v1"));

//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    var importService = services.GetRequiredService<DataImportService>();
//    var jsonPath = Path.Combine(AppContext.BaseDirectory, "App_Data", "IpSelect_tw.json");
//    await importService.ImportFromJsonAsync(jsonPath);
//}



// 启用控制器路由
app.UseCors("AllowVueApp");
app.MapControllers();
app.Run();



