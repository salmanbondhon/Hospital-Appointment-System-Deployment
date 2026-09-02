using HospitalAPI.DTOs;
using HospitalAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using HospitalAPI.Responses;


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


        // =========================
        // CHANGE PASSWORD
        // =========================

        [Authorize]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword(
            ChangePasswordDto dto)
        {
            var userIdClaim =
                User.FindFirst(
                    ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }


            int userId =
                int.Parse(userIdClaim.Value);


            await _service.ChangePasswordAsync(
                userId,
                dto);


            return Ok(
                new ApiResponse<object>
                {
                    Success = true,

                    Message =
                        "Password changed successfully.",

                    Data = null
                });
        }
    }
}