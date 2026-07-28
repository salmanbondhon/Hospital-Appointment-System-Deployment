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
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService _service;

        public DoctorController(IDoctorService service)
        {
            _service = service;
        }

        // GET: api/Doctor
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var doctors = await _service.GetAllAsync();
            return Ok(doctors);
        }

        // GET: api/Doctor/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var doctor = await _service.GetByIdAsync(id);

            if (doctor == null)
                return NotFound();

            return Ok(doctor);
        }

        // POST: api/Doctor
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateDoctorDto dto)
        {
            var doctor = await _service.AddAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = doctor.Id },
                doctor);
        }

        // PUT: api/Doctor/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateDoctorDto dto)
        {
            await _service.UpdateAsync(id, dto);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Doctor updated successfully.",
                Data = null
            });
        }

        // DELETE: api/Doctor/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Doctor deleted successfully.",
                Data = null
            });
        }
    }
}