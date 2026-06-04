using System.Net;
using System.Text.Json;

namespace VNC.Api.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next; // Luân chuyển request sang thành phần tiếp theo
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Cho phép request tiếp tục đi sâu vào hệ thống (đến Controller)
                await _next(context);
            }
            catch (Exception ex)
            {
                // Nếu có bất kỳ lỗi nào xảy ra ở tầng dưới văng lên, xử lý tại đây
                _logger.LogError(ex, "Một lỗi không mong muốn đã xảy ra: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            // Mặc định là lỗi 500 (Lỗi hệ thống máy chủ)
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // Mẹo hay: Nếu là lỗi do bạn chủ động quăng ra ở Service (như Hết hàng, Sai ID), hãy chuyển nó thành lỗi 400 Bad Request
            if (exception is Exception && exception.Message.Contains("không tồn tại") || exception.Message.Contains("không đủ tồn kho"))
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }

            var response = new
            {
                Success = false,
                StatusCode = context.Response.StatusCode,
                Message = exception.Message,
                Detail = "Vui lòng liên hệ Admin hệ thống để biết thêm chi tiết."
            };

            var jsonResponse = JsonSerializer.Serialize(response);
            return context.Response.WriteAsync(jsonResponse);
        }
    }
}
