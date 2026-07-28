using HospitalAPI.DTOs;
using HospitalAPI.Interfaces;
using HospitalAPI.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace HospitalAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _service;

        public PatientController(IPatientService service)
        {
            _service = service;
        }

        // GET: api/Patient
        [Authorize(Roles = "Admin,Doctor")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var patients = await _service.GetAllAsync();
            return Ok(patients);
        }

        // GET: api/Patient/5
        [Authorize(Roles = "Admin,Doctor")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var patient = await _service.GetByIdAsync(id);

            if (patient == null)
                return NotFound();

            return Ok(patient);
        }

        // POST: api/Patient
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreatePatientDto dto)
        {
            var patient = await _service.AddAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = patient.Id },
                patient);
        }

        // PUT: api/Patient/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdatePatientDto dto)
        {
            await _service.UpdateAsync(id, dto);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Patient updated successfully.",
                Data = null
            });
        }

        // DELETE: api/Patient/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Patient deleted successfully.",
                Data = null
            });
        }
    }
}