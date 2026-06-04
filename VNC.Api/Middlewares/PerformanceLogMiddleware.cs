using System.Diagnostics;

namespace VNC.Api.Middlewares
{
    public class PerformanceLogMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<PerformanceLogMiddleware> _logger;

        public PerformanceLogMiddleware(RequestDelegate next, ILogger<PerformanceLogMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            // Để request chạy qua Controller xử lý
            await _next(context);

            stopwatch.Stop();
            var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

            // Nếu API chạy tốn hơn 500ms, lập tức in cảnh báo (Warning) ra màn hình Console để bạn tối ưu hóa
            if (elapsedMilliseconds > 500)
            {
                _logger.LogWarning("⚠️ API CẢNH BÁO CHẬM: {Method} {Path} chạy mất {Elapsed} ms",
                    context.Request.Method, context.Request.Path, elapsedMilliseconds);
            }
            else
            {
                _logger.LogInformation("API {Method} {Path} hoàn thành trong {Elapsed} ms",
                    context.Request.Method, context.Request.Path, elapsedMilliseconds);
            }
        }
    }
}
