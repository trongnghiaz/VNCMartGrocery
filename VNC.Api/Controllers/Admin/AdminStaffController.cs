using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VNC.Application.Interfaces;
using VNC.Application.Models;

namespace VNC.Api.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/staffs")]
    [Authorize(Roles = "Admin")]
    public class AdminStaffController : ControllerBase
    {
        private readonly IAppDbContext _context;

        public AdminStaffController(IAppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetStaffs()
        {
            var result = await _context.Staffs
                .Include(s => s.Role)
                .OrderByDescending(s => s.CreatedAt)
                .Select(s => new
                {
                    s.StaffId,
                    s.FullName,
                    s.Email,
                    s.IsActive,
                    s.CreatedAt,
                    s.RoleId,
                    RoleName = s.Role.RoleName
                })
                .ToListAsync();

            return Ok(ApiResponse<object>.Success(result));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var staff = await _context.Staffs
                .Include(s => s.Role)
                .Where(s => s.StaffId == id)
                .Select(s => new
                {
                    s.StaffId,
                    s.FullName,
                    s.Email,
                    s.IsActive,
                    s.CreatedAt,
                    s.RoleId,
                    RoleName = s.Role.RoleName
                })
                .FirstOrDefaultAsync();

            if (staff == null)
            {
                return NotFound(ApiResponse<object>.Failure("Không tìm thấy nhân viên."));
            }

            return Ok(ApiResponse<object>.Success(staff));
        }

        [HttpPut("{id}/lock")]
        public async Task<IActionResult> LockStaff(int id)
        {
            var staff = await _context.Staffs.FirstOrDefaultAsync(s => s.StaffId == id);

            if (staff == null)
            {
                return NotFound(ApiResponse<bool>.Failure("Không tìm thấy nhân viên để khóa."));
            }

            staff.IsActive = false;
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<bool>.Success(true));
        }

        [HttpPut("{id}/unlock")]
        public async Task<IActionResult> UnlockStaff(int id)
        {
            var staff = await _context.Staffs.FirstOrDefaultAsync(s => s.StaffId == id);

            if (staff == null)
            {
                return NotFound(ApiResponse<bool>.Failure("Không tìm thấy nhân viên để mở khóa."));
            }

            staff.IsActive = true;
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<bool>.Success(true));
        }
    }
}