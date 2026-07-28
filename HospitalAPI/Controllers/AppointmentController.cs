using HospitalAPI.DTOs;
using HospitalAPI.Interfaces;
using HospitalAPI.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace HospitalAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _service;

        public AppointmentController(IAppointmentService service)
        {
            _service = service;
        }

        // GET: api/Appointment
        [Authorize(Roles = "Admin,Doctor,Patient")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            int userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            string role = User.FindFirst(ClaimTypes.Role)!.Value;

            var appointments = await _service.GetAllAsync(userId, role);

            return Ok(appointments);
        }

        // GET: api/Appointment/5
        [Authorize(Roles = "Admin,Doctor,Patient")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            int userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            string role = User.FindFirst(ClaimTypes.Role)!.Value;

            var appointment = await _service.GetByIdAsync(id, userId, role);

            if (appointment == null)
                return NotFound();

            return Ok(appointment);
        }

        // POST: api/Appointment
        [Authorize(Roles = "Admin,Patient")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateAppointmentDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim.Value);

            var appointment = await _service.AddAsync(dto, userId);

            return Ok(new ApiResponse<AppointmentDto>
            {
                Success = true,
                Message = "Appointment created successfully.",
                Data = appointment
            });
        }

        // PUT: api/Appointment/5
        [Authorize(Roles = "Admin,Doctor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateAppointmentDto dto)
        {
            int userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            string role = User.FindFirst(ClaimTypes.Role)!.Value;

            await _service.UpdateAsync(id, dto, userId, role);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Appointment updated successfully.",
                Data = null
            });
        }

        // DELETE: api/Appointment/5
        [Authorize(Roles = "Admin,Doctor")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            int userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            string role = User.FindFirst(ClaimTypes.Role)!.Value;

            await _service.DeleteAsync(id, userId, role);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Appointment deleted successfully.",
                Data = null
            });
        }
    }
}