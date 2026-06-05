
namespace VNC.Application.Models
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public T Data { get; set; }
        public string ErrorMessage { get; set; }
        public int StatusCode { get; set; }

        public static ApiResponse<T> Success(T data, int statusCode = 200)
            => new() { IsSuccess = true, Data = data, StatusCode = statusCode };

        public static ApiResponse<T> Failure(string error, int statusCode = 400)
            => new() { IsSuccess = false, ErrorMessage = error, StatusCode = statusCode };
    }
}
