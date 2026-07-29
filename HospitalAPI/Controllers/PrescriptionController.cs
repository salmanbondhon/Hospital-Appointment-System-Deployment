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

        public PrescriptionController(IPrescriptionService service)
        {
            _service = service;
        }

        // ===========================
        // Create Prescription
        // Doctor Only
        // ===========================
        [Authorize(Roles = "Doctor")]
        [HttpPost]
        public async Task<IActionResult> Create(CreatePrescriptionDto dto)
        {
            int userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var prescription = await _service.CreateAsync(dto, userId);

            return Ok(new
            {
                Success = true,
                Message = "Prescription created successfully.",
                Data = prescription
            });
        }

        // ===========================
        // Get All Prescriptions
        // ===========================
        [Authorize(Roles = "Admin,Doctor,Patient")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            int userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            string role = User.FindFirst(ClaimTypes.Role)!.Value;

            var prescriptions = await _service.GetAllAsync(userId, role);

            return Ok(prescriptions);
        }

        // ===========================
        // Get Prescription By Id
        // ===========================
        [Authorize(Roles = "Admin,Doctor,Patient")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            int userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            string role = User.FindFirst(ClaimTypes.Role)!.Value;

            var prescription = await _service.GetByIdAsync(id, userId, role);

            if (prescription == null)
                return NotFound();

            return Ok(prescription);
        }

        // ===========================
        // Delete Prescription
        // Admin Only
        // ===========================
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);

            return Ok(new
            {
                Success = true,
                Message = "Prescription deleted successfully."
            });
        }
    }
}