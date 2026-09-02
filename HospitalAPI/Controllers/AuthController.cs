using HospitalAPI.DTOs;
using HospitalAPI.Interfaces;
using HospitalAPI.Responses;
using Microsoft.AspNetCore.Mvc;

namespace HospitalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _service;

        public AuthController(IUserService service)
        {
            _service = service;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            await _service.RegisterAsync(dto);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "User registered successfully.",
                Data = null
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await _service.LoginAsync(dto);

            return Ok(new ApiResponse<LoginResponseDto>
            {
                Success = true,
                Message = "Login successful.",
                Data = result
            });
        }


        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(
    ForgotPasswordDto dto)
        {
            await _service.ForgotPasswordAsync(dto);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message =
                    "If an account exists for this email, a password reset link has been sent.",
                Data = null
            });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(
    ResetPasswordDto dto)
        {
            await _service.ResetPasswordAsync(dto);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Password reset successfully.",
                Data = null
            });
        }
    }
}