using System.Security.Claims;
using HospitalAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Doctor,Patient")]
    public class MedicalRecordController : ControllerBase
    {
        private readonly IMedicalRecordService _service;

        public MedicalRecordController(
            IMedicalRecordService service)
        {
            _service = service;
        }

        // =========================================
        // GET PATIENT MEDICAL HISTORY
        // =========================================

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetPatientHistory(
            int patientId)
        {
            int userId = int.Parse(
                User.FindFirst(
                    ClaimTypes.NameIdentifier)!.Value);

            string role = User.FindFirst(
                ClaimTypes.Role)!.Value;

            var history =
                await _service.GetPatientHistoryAsync(
                    patientId,
                    userId,
                    role);

            return Ok(history);
        }
    }
}