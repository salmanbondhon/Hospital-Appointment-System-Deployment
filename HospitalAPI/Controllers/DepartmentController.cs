
using HospitalAPI.DTOs;
using HospitalAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace HospitalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _service;

        public DepartmentController(IDepartmentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var department = await _service.GetByIdAsync(id);

            if (department == null)
                return NotFound();

            return Ok(department);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateDepartmentDto dto)
        {
            var department = await _service.AddAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = department.Id },
                department);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateDepartmentDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);

            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
