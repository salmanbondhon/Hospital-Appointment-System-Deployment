using HospitalAPI.DTOs;
using HospitalAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service;
        }


        // =========================
        // GET AVAILABLE DOCTOR USERS
        // =========================

        [Authorize(Roles = "Admin")]
        [HttpGet("available-doctors")]
        public async Task<IActionResult> GetAvailableDoctorUsers()
        {
            var users =
                await _service.GetAvailableDoctorUsersAsync();

            return Ok(users);
        }

        // =========================
        // GET AVAILABLE PATIENT USERS
        // =========================

        [Authorize(Roles = "Admin")]
        [HttpGet("available-patients")]
        public async Task<IActionResult> GetAvailablePatientUsers()
        {
            var users =
                await _service.GetAvailablePatientUsersAsync();

            return Ok(users);
        }
    }
}