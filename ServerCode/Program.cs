using AutoMapper;
using Repositories;
using ServerCode;
using ServerCode.DAO;
using ServerCode.DTO;


//using ServerCode.Repositories;

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
builder.Services.AddSingleton(new FileLogger("../../asd.txt"));
string connection = builder.Configuration.GetConnectionString("DefaultConnection")!;

#region SetMapper
(Type, Type)[] types =
{
    (typeof(PlayerDTO),typeof(PlayerVO)),
    (typeof(PlayerDataDTO),typeof(PlayerDataVO)),
    (typeof(PlayerItemDTO),typeof(PlayerItemVO)),
    (typeof(AuctionItemDTO),typeof(AuctionItemVO)),
};
var config = new MapperConfiguration(expression =>
{
    foreach (var item in types)
    {
        Type? dtoType = item.Item1;
        Type? daoType = item.Item2;
        expression!.CreateMap(dtoType, daoType);
        expression.CreateMap(daoType, dtoType);
    }
});

#endregion

builder.Services.AddScoped(provider => new ServiceManager(config.CreateMapper(), connection));
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://0.0.0.0", "https://example.com")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});
builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(3303);
    options.Limits.MaxConcurrentConnections = 100;  // 동시 연결 최대 100개
    options.Limits.MaxConcurrentUpgradedConnections = 100;
    options.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(30);
});
builder.WebHost.UseKestrel();
var app = builder.Build();
app.UseRouting();
app.UseCors("AllowAllOrigins");
app.UseSession();
//app.UseHttpsRedirection(); // HTTP 요청을 HTTPS로 리디렉트
app.MapControllers();

app.Run();
