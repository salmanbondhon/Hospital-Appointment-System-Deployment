using HospitalAPI.DTOs;
using HospitalAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HospitalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;


        public PaymentController(
            IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }


        // =================================================
        // CURRENT USER ID
        // =================================================

        private int UserId =>
            int.Parse(
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier)!);


        // =================================================
        // CURRENT USER ROLE
        // =================================================

        private string Role =>
            User.FindFirstValue(
                ClaimTypes.Role)!;


        // =================================================
        // GET ALL
        // =================================================

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result =
                await _paymentService.GetAllAsync(
                    UserId,
                    Role);

            return Ok(result);
        }


        // =================================================
        // GET BY ID
        // =================================================

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(
            int id)
        {
            var result =
                await _paymentService.GetByIdAsync(
                    id,
                    UserId,
                    Role);


            if (result == null)
            {
                return NotFound();
            }


            return Ok(result);
        }


        // =================================================
        // CREATE PAYMENT
        // PATIENT ONLY
        // =================================================

        [Authorize(Roles = "Patient")]
        [HttpPost]
        public async Task<IActionResult> Create(
            CreatePaymentDto dto)
        {
            var result =
                await _paymentService.CreateAsync(
                    dto,
                    UserId);


            return Ok(result);
        }


        // =================================================
        // UPDATE PAYMENT
        // ADMIN ONLY
        // =================================================

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            UpdatePaymentDto dto)
        {
            await _paymentService.UpdateAsync(
                id,
                dto);


            return NoContent();
        }


        // =================================================
        // DELETE PAYMENT
        // ADMIN ONLY
        // =================================================

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(
            int id)
        {
            await _paymentService.DeleteAsync(
                id);


            return NoContent();
        }
    }
}