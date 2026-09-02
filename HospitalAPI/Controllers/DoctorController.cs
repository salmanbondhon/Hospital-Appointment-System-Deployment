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
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService _service;

        public DoctorController(
            IDoctorService service)
        {
            _service = service;
        }


        // =====================================================
        // GET ALL
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var doctors =
                await _service.GetAllAsync();

            return Ok(doctors);
        }


        // =====================================================
        // GET CURRENT LOGGED-IN DOCTOR
        // =====================================================

        [Authorize(Roles = "Doctor")]
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userIdClaim =
                User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            int userId =
                int.Parse(userIdClaim.Value);

            var doctor =
                await _service.GetMyProfileAsync(userId);

            if (doctor == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,

                    Message =
                        "Doctor profile not found.",

                    Data = null
                });
            }

            return Ok(doctor);
        }

        // =====================================================
        // GET BY ID
        // =====================================================

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(
            int id)
        {
            var doctor =
                await _service.GetByIdAsync(id);

            if (doctor == null)
            {
                return NotFound();
            }

            return Ok(doctor);
        }


       



        // =====================================================
        // CREATE
        // =====================================================

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(
            CreateDoctorDto dto)
        {
            var doctor =
                await _service.AddAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = doctor.Id },
                doctor);
        }


        // =====================================================
        // UPDATE
        // =====================================================

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            UpdateDoctorDto dto)
        {
            await _service.UpdateAsync(
                id,
                dto);

            return Ok(
                new ApiResponse<object>
                {
                    Success = true,

                    Message =
                        "Doctor updated successfully.",

                    Data = null
                });
        }


        // =====================================================
        // DELETE
        // =====================================================

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(
            int id)
        {
            await _service.DeleteAsync(id);

            return Ok(
                new ApiResponse<object>
                {
                    Success = true,

                    Message =
                        "Doctor deleted successfully.",

                    Data = null
                });
        }
    }
}