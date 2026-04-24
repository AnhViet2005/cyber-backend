using ConnectDB.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ConnectDB.Hubs;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    // 👉 Sử dụng PostgreSQL (đã chuyển sang Npgsql)
    var pgConnection = builder.Configuration.GetConnectionString("PostgreSqlConnection");
    options.UseNpgsql(pgConnection);
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// 🟢 Cấu hình SignalR
builder.Services.AddSignalR();

// 🟢 Cấu hình CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowNextApp",
        policy => policy.WithOrigins("http://localhost:3000", "http://localhost:3001") // Hỗ trợ cả 2 web
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
});


// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 🔐 Cấu hình JWT
var jwtKey = "PhimSecNhatBanVip12345678901234567890"; // Secret key (nên để trong appsettings.json)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

var app = builder.Build();

// Tự động áp dụng Migration và Seed dữ liệu khi khởi động
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // db.Database.Migrate(); 
    
    // 🚀 Tự động nạp tồn kho 100 cho tất cả món (trừ Giờ chơi, Combo, Gói giờ) đang bị hết hàng
    var emptyStockServices = await db.Services.Where(s => s.StockQuantity == 0 && s.Category != "Time" && s.Category != "Combos" && s.Category != "Packages").ToListAsync();
    if (emptyStockServices.Any())
    {
        foreach (var service in emptyStockServices)
        {
            service.StockQuantity = 100;
        }
        await db.SaveChangesAsync();
    }
}

// Middleware
//if (app.Environment.IsDevelopment())

    app.UseSwagger();
    app.UseSwaggerUI();


app.UseAuthentication(); // 🔐 Thêm Auth
app.UseAuthorization();

// 🟢 Kích hoạt CORS
app.UseCors("AllowNextApp");

// 📁 Cho phép đọc file tĩnh (ảnh)
app.UseStaticFiles();


app.MapControllers();
app.MapHub<ChatHub>("/chatHub");

app.Run();