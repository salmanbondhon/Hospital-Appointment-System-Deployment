using HospitalAPI.DTOs;
using HospitalAPI.Interfaces;
using HospitalAPI.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HospitalAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _service;

        public AppointmentController(
            IAppointmentService service)
        {
            _service = service;
        }


        // =====================================================
        // GET ALL
        // =====================================================

        [Authorize(Roles = "Admin,Doctor,Patient")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            int userId = int.Parse(
                User.FindFirst(
                    ClaimTypes.NameIdentifier)!.Value);

            string role =
                User.FindFirst(
                    ClaimTypes.Role)!.Value;

            var appointments =
                await _service.GetAllAsync(
                    userId,
                    role);

            return Ok(appointments);
        }


        // =====================================================
        // GET BY ID
        // =====================================================

        [Authorize(Roles = "Admin,Doctor,Patient")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            int userId = int.Parse(
                User.FindFirst(
                    ClaimTypes.NameIdentifier)!.Value);

            string role =
                User.FindFirst(
                    ClaimTypes.Role)!.Value;

            var appointment =
                await _service.GetByIdAsync(
                    id,
                    userId,
                    role);

            if (appointment == null)
            {
                return NotFound();
            }

            return Ok(appointment);
        }


        // =====================================================
        // CREATE
        // =====================================================

        // Patient -> creates appointment for himself
        // Admin   -> creates appointment for selected patient
        // Doctor  -> cannot create appointment

        [Authorize(Roles = "Admin,Patient")]
        [HttpPost]
        public async Task<IActionResult> Create(
            CreateAppointmentDto dto)
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

            string role =
                User.FindFirst(
                    ClaimTypes.Role)!.Value;

            var appointment =
                await _service.AddAsync(
                    dto,
                    userId,
                    role);

            return Ok(
                new ApiResponse<AppointmentDto>
                {
                    Success = true,
                    Message =
                        "Appointment created successfully.",
                    Data = appointment
                });
        }


        // =====================================================
        // UPDATE
        // =====================================================

        [Authorize(Roles = "Admin,Doctor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            UpdateAppointmentDto dto)
        {
            int userId = int.Parse(
                User.FindFirst(
                    ClaimTypes.NameIdentifier)!.Value);

            string role =
                User.FindFirst(
                    ClaimTypes.Role)!.Value;

            await _service.UpdateAsync(
                id,
                dto,
                userId,
                role);

            return Ok(
                new ApiResponse<object>
                {
                    Success = true,
                    Message =
                        "Appointment updated successfully.",
                    Data = null
                });
        }


        // =====================================================
        // DELETE
        // =====================================================

        [Authorize(Roles = "Admin,Doctor")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(
            int id)
        {
            int userId = int.Parse(
                User.FindFirst(
                    ClaimTypes.NameIdentifier)!.Value);

            string role =
                User.FindFirst(
                    ClaimTypes.Role)!.Value;

            await _service.DeleteAsync(
                id,
                userId,
                role);

            return Ok(
                new ApiResponse<object>
                {
                    Success = true,
                    Message =
                        "Appointment deleted successfully.",
                    Data = null
                });
        }


        // =====================================================
        // APPROVE
        // =====================================================

        [Authorize(Roles = "Admin,Doctor")]
        [HttpPut("{id}/approve")]
        public async Task<IActionResult>
            ApproveAppointment(int id)
        {
            int userId = int.Parse(
                User.FindFirst(
                    ClaimTypes.NameIdentifier)!.Value);

            string role =
                User.FindFirst(
                    ClaimTypes.Role)!.Value;

            await _service.ApproveAppointmentAsync(
                id,
                userId,
                role);

            return Ok(
                new ApiResponse<object>
                {
                    Success = true,
                    Message =
                        "Appointment approved successfully.",
                    Data = null
                });
        }


        // =====================================================
        // COMPLETE
        // =====================================================

        [Authorize(Roles = "Admin,Doctor")]
        [HttpPut("{id}/complete")]
        public async Task<IActionResult>
            CompleteAppointment(int id)
        {
            int userId = int.Parse(
                User.FindFirst(
                    ClaimTypes.NameIdentifier)!.Value);

            string role =
                User.FindFirst(
                    ClaimTypes.Role)!.Value;

            await _service.CompleteAppointmentAsync(
                id,
                userId,
                role);

            return Ok(
                new ApiResponse<object>
                {
                    Success = true,
                    Message =
                        "Appointment completed successfully.",
                    Data = null
                });
        }


        // =====================================================
        // CANCEL
        // =====================================================

        // Patient can cancel own appointment
        // Doctor can cancel own appointment
        // Admin can cancel any appointment

        [Authorize(Roles = "Admin,Doctor,Patient")]
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult>
            CancelAppointment(int id)
        {
            int userId = int.Parse(
                User.FindFirst(
                    ClaimTypes.NameIdentifier)!.Value);

            string role =
                User.FindFirst(
                    ClaimTypes.Role)!.Value;

            await _service.CancelAppointmentAsync(
                id,
                userId,
                role);

            return Ok(
                new ApiResponse<object>
                {
                    Success = true,
                    Message =
                        "Appointment cancelled successfully.",
                    Data = null
                });
        }
    }
}