using Microsoft.Extensions.DependencyInjection;
using Repositories;
//using ServerCode.Repositories;
using System.Xml.Linq;

var builder = WebApplication.CreateBuilder(args);
// 🔹 세션을 위한 메모리 캐시 추가
builder.Services.AddDistributedMemoryCache();

// 🔹 데이터 보호 서비스 추가 (세션 오류 방지)
builder.Services.AddDataProtection();

// 🔹 세션 서비스 추가
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // 세션 유지 시간
    options.Cookie.HttpOnly = true; // 보안 강화
    options.Cookie.IsEssential = true; // 필수 쿠키로 설정
});
builder.Services.AddScoped(provider => new DBManager($"Server=127.0.0.1;Port=3306;Database=opentutorials;Uid=root;Pwd=1652;Pooling=true"));
//DBManager.Instance.ConnectDB($"Server=127.0.0.1;Port=3306;Database=opentutorials;Uid=root;Pwd=1652;Pooling=true");
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost", "https://example.com")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});
builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();
builder.WebHost.UseIISIntegration();
var app = builder.Build();

app.UseRouting();
app.UseCors("AllowAllOrigins");
app.UseSession();
app.MapControllers();

app.Run();
