using System.Security.Claims;
using HospitalAPI.DTOs;
using HospitalAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrescriptionController : ControllerBase
    {
        private readonly IPrescriptionService _service;

        public PrescriptionController(
            IPrescriptionService service)
        {
            _service = service;
        }


        // ===========================
        // CREATE PRESCRIPTION
        // Doctor Only
        // ===========================
        [Authorize(Roles = "Doctor")]
        [HttpPost]
        public async Task<IActionResult> Create(
            CreatePrescriptionDto dto)
        {
            int userId = int.Parse(
                User.FindFirst(
                    ClaimTypes.NameIdentifier)!.Value);

            var prescription =
                await _service.CreateAsync(
                    dto,
                    userId);

            return Ok(new
            {
                Success = true,
                Message =
                    "Prescription created successfully.",
                Data = prescription
            });
        }


        // ===========================
        // GET ALL PRESCRIPTIONS
        // Admin + Doctor + Patient
        // ===========================
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

            var prescriptions =
                await _service.GetAllAsync(
                    userId,
                    role);

            return Ok(prescriptions);
        }


        // ===========================
        // GET PRESCRIPTION BY ID
        // Admin + Doctor + Patient
        // ===========================
        [Authorize(Roles = "Admin,Doctor,Patient")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(
            int id)
        {
            int userId = int.Parse(
                User.FindFirst(
                    ClaimTypes.NameIdentifier)!.Value);

            string role =
                User.FindFirst(
                    ClaimTypes.Role)!.Value;

            var prescription =
                await _service.GetByIdAsync(
                    id,
                    userId,
                    role);

            if (prescription == null)
                return NotFound();

            return Ok(prescription);
        }


        // ===========================
        // UPDATE PRESCRIPTION
        // Admin + Doctor
        // ===========================
        [Authorize(Roles = "Admin,Doctor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            UpdatePrescriptionDto dto)
        {
            int userId = int.Parse(
                User.FindFirst(
                    ClaimTypes.NameIdentifier)!.Value);

            string role =
                User.FindFirst(
                    ClaimTypes.Role)!.Value;

            var prescription =
                await _service.UpdateAsync(
                    id,
                    dto,
                    userId,
                    role);

            return Ok(new
            {
                Success = true,
                Message =
                    "Prescription updated successfully.",
                Data = prescription
            });
        }


        // ===========================
        // DELETE PRESCRIPTION
        // Admin + Doctor
        // ===========================
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

            return Ok(new
            {
                Success = true,
                Message =
                    "Prescription deleted successfully."
            });
        }
    }
}