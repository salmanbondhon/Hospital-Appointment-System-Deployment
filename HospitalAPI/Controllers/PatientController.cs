using System.Security.Claims;
using HospitalAPI.DTOs;
using HospitalAPI.Interfaces;
using HospitalAPI.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _service;

        public PatientController(
            IPatientService service)
        {
            _service = service;
        }


        // =================================================
        // GET ALL
        // =================================================

        [Authorize(Roles = "Admin,Doctor")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var patients =
                await _service.GetAllAsync();

            return Ok(patients);
        }


        // =================================================
        // GET BY ID
        // =================================================

        [Authorize(Roles = "Admin,Doctor")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(
            int id)
        {
            var patient =
                await _service.GetByIdAsync(id);

            if (patient == null)
            {
                return NotFound();
            }

            return Ok(patient);
        }


        // =================================================
        // CREATE
        // ADMIN + PATIENT
        // =================================================

        [Authorize(Roles = "Admin,Patient")]
        [HttpPost]
        public async Task<IActionResult> Create(
            CreatePatientDto dto)
        {
            var userIdClaim =
                User.FindFirst(
                    ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }


            int currentUserId =
                int.Parse(userIdClaim.Value);


            var currentUserRole =
                User.FindFirst(
                    ClaimTypes.Role)?.Value
                ?? string.Empty;


            var patient =
                await _service.AddAsync(
                    dto,
                    currentUserId,
                    currentUserRole);


            return CreatedAtAction(
                nameof(GetById),
                new { id = patient.Id },
                patient);
        }


        // =================================================
        // UPDATE
        // ADMIN + PATIENT
        // =================================================

        [Authorize(Roles = "Admin,Patient")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            UpdatePatientDto dto)
        {
            var userIdClaim =
                User.FindFirst(
                    ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }


            int currentUserId =
                int.Parse(userIdClaim.Value);


            var currentUserRole =
                User.FindFirst(
                    ClaimTypes.Role)?.Value
                ?? string.Empty;


            await _service.UpdateAsync(
                id,
                dto,
                currentUserId,
                currentUserRole);


            return Ok(new ApiResponse<object>
            {
                Success = true,

                Message =
                    "Patient updated successfully.",

                Data = null
            });
        }


        // =================================================
        // DELETE
        // ADMIN ONLY
        // =================================================

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(
            int id)
        {
            await _service.DeleteAsync(id);

            return Ok(new ApiResponse<object>
            {
                Success = true,

                Message =
                    "Patient deleted successfully.",

                Data = null
            });
        }
    }
}