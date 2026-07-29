using System.Security.Claims;
using HospitalAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _service;

        public DashboardController(IDashboardService service)
        {
            _service = service;
        }

        // =============================
        // Admin Dashboard
        // =============================
        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public async Task<IActionResult> GetAdminDashboard()
        {
            var dashboard = await _service.GetAdminDashboardAsync();

            return Ok(dashboard);
        }

        // =============================
        // Doctor Dashboard
        // =============================
        [Authorize(Roles = "Doctor")]
        [HttpGet("doctor")]
        public async Task<IActionResult> GetDoctorDashboard()
        {
            int userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var dashboard = await _service.GetDoctorDashboardAsync(userId);

            return Ok(dashboard);
        }

        // =============================
        // Patient Dashboard
        // =============================
        [Authorize(Roles = "Patient")]
        [HttpGet("patient")]
        public async Task<IActionResult> GetPatientDashboard()
        {
            int userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var dashboard = await _service.GetPatientDashboardAsync(userId);

            return Ok(dashboard);
        }
    }
}
