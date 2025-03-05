using ServerCode.Core;
using ServerCode.Data;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql;
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

// DBContext 설정
builder.Services.AddDbContext<GameDbContext>(options =>
    options.UseMySql(
        "Server=127.0.0.1;Port=3306;Database=opentutorials;Uid=root;Pwd=1652;Pooling=true",
        ServerVersion.AutoDetect("Server=127.0.0.1;Port=3306;Database=opentutorials;Uid=root;Pwd=1652;Pooling=true")
    )
);

DBManager.Instance.ConnectDB("Server=127.0.0.1;Port=3306;Database=opentutorials;Uid=root;Pwd=1652;Pooling=true");

builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();
builder.WebHost.UseIISIntegration();
var app = builder.Build();

app.UseRouting();
app.UseSession();
app.MapControllers();

app.Run();
