using System.Security.Claims;
using HospitalAPI.DTOs;
using HospitalAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorLeaveController : ControllerBase
    {
        private readonly IDoctorLeaveService _service;

        public DoctorLeaveController(IDoctorLeaveService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Doctor")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateLeaveDto dto)
        {
            int userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var leave = await _service.CreateLeaveAsync(dto, userId);

            return Ok(new
            {
                Success = true,
                Message = "Leave request submitted successfully.",
                Data = leave
            });
        }

        [Authorize(Roles = "Doctor")]
        [HttpGet("my-leaves")]
        public async Task<IActionResult> MyLeaves()
        {
            int userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var leaves = await _service.GetMyLeavesAsync(userId);

            return Ok(leaves);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var leaves = await _service.GetAllAsync();

            return Ok(leaves);
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            await _service.ApproveLeaveAsync(id);

            return Ok(new
            {
                Success = true,
                Message = "Leave approved successfully."
            });
        }



        [Authorize(Roles = "Admin,Doctor")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            int userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            string role = User.FindFirst(ClaimTypes.Role)!.Value;

            await _service.DeleteLeaveAsync(id, userId, role);

            return Ok(new
            {
                Success = true,
                Message = "Leave deleted successfully."
            });
        }
    }
}