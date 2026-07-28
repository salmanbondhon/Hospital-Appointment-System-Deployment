
using HospitalAPI.DTOs;
using HospitalAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;
using HospitalAPI.Responses;
using Microsoft.AspNetCore.Authorization;




namespace HospitalAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _service;

        public DepartmentController(IDepartmentService service)
        {
            _service = service;
        }

        // GET: api/Department
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var departments = await _service.GetAllAsync();

            return Ok(new ApiResponse<IEnumerable<DepartmentDto>>
            {
                Success = true,
                Message = "Departments retrieved successfully.",
                Data = departments
            });
        }

        // GET: api/Department/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var department = await _service.GetByIdAsync(id);

            if (department == null)
                return NotFound();

            return Ok(new ApiResponse<DepartmentDto>
            {
                Success = true,
                Message = "Department retrieved successfully.",
                Data = department
            });
        }

        // POST: api/Department
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateDepartmentDto dto)
        {
            var department = await _service.AddAsync(dto);

            return Ok(new ApiResponse<DepartmentDto>
            {
                Success = true,
                Message = "Department created successfully.",
                Data = department
            });
        }

        // PUT: api/Department/1
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateDepartmentDto dto)
        {
            await _service.UpdateAsync(id, dto);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Department updated successfully.",
                Data = null
            });
        }

        // DELETE: api/Department/1
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Department deleted successfully.",
                Data = null
            });
        }
    }
}
