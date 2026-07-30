using HospitalAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HospitalAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestEmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public TestEmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost]
        public async Task<IActionResult> SendTestEmail()
        {
            await _emailService.SendEmailAsync(
                "bandhansalman@gmail.com",
                "Hospital API Test",
                "<h2>Email is working successfully!</h2><p>This email was sent from your Hospital Management System.</p>");

            return Ok("Email sent successfully.");
        }
    }
}