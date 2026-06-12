

namespace VNC.Application.Interfaces
{
    public interface IUserContextService
    {
        int? GetUserId();
        string? GetUserRole();
    }
}
