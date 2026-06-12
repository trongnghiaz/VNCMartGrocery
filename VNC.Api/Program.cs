using Serilog;
using VNC.Api.Factories;
using VNC.Api.Middlewares;
using VNC.Application;
using VNC.Application.Models;
using VNC.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Host.UseSerilog();
// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new EnumerationJsonConverterFactory());
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "VNC Mart API", Version = "v1" });

    // Cấu hình định nghĩa cơ chế bảo mật JWT cho Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Điền chuỗi JWT Token của bạn theo định dạng: Bearer {chuỗi_token_của_bạn}",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


// =======================================================
// ĐĂNG KÝ DI CỦA CÁC TẦNG KHÁC VÀO HỆ THỐNG TẠI ĐÂY
// =======================================================
builder.Services.Configure<AppSettings>(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration); // Kích hoạt tầng Infrastructure
builder.Services.AddApplicationServices();                         // Kích hoạt tầng Application
// =======================================================


var app = builder.Build();


app.UseSerilogRequestLogging();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<PerformanceLogMiddleware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Map("/ping", async (HttpContext context) =>
{
    context.Response.ContentType = "text/plain";
    await context.Response.WriteAsync("Pong! Server đang chạy mượt mà tại thời điểm: " + DateTime.Now);
});
app.Run();
