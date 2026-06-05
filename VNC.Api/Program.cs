using Serilog;
using VNC.Api.Factories;
using VNC.Api.Middlewares;
using VNC.Application;
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
builder.Services.AddSwaggerGen();


// =======================================================
// ĐĂNG KÝ DI CỦA CÁC TẦNG KHÁC VÀO HỆ THỐNG TẠI ĐÂY
// =======================================================
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

app.UseAuthorization();

app.MapControllers();
app.Map("/ping", async (HttpContext context) =>
{
    context.Response.ContentType = "text/plain";
    await context.Response.WriteAsync("Pong! Server đang chạy mượt mà tại thời điểm: " + DateTime.Now);
});
app.Run();
