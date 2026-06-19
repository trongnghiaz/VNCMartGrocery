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
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Một lỗi không mong muốn đã xảy ra: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            // 1. PHÂN LOẠI MÃ LỖI HTTP DỰA TRÊN KIỂU EXCEPTION HOẶC MESSAGE
            if (exception is UnauthorizedAccessException)
            {
                // Trả về mã 403 Forbidden khi nhân viên chưa có Role
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            }
            else if (exception is ArgumentException ||
                     exception is InvalidOperationException ||
                     exception.Message.Contains("không tồn tại") ||
                     exception.Message.Contains("không đủ tồn kho"))
            {
                // Giữ nguyên logic cũ của bạn: Chuyển các lỗi nghiệp vụ thông thường thành 400 Bad Request
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            else
            {
                // Mặc định cho các lỗi hệ thống không lường trước được (NullReference, sập DB...)
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            // 2. ĐÓNG GÓI LẠI ĐỊNH DẠNG PHẢN HỒI (Giữ nguyên cấu trúc object của bạn)
            var response = new
            {
                Success = false,
                StatusCode = context.Response.StatusCode,
                Message = exception.Message,
                Detail = context.Response.StatusCode == (int)HttpStatusCode.InternalServerError
                    ? "Vui lòng liên hệ Admin hệ thống để biết thêm chi tiết."
                    : "Lỗi xử lý nghiệp vụ hệ thống." // Tinh chỉnh nhẹ để thông báo thân thiện hơn tùy loại lỗi
            };

            // Lưu ý: Đảm bảo sử dụng CamelCase nếu dự án của bạn đang dùng cấu trúc chữ thường cho Frontend
            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            return context.Response.WriteAsync(jsonResponse);
        }
    }
}
