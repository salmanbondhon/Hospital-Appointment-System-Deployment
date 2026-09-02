using System.Security.Claims;
using HospitalAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _service;

        public NotificationController(
            INotificationService service)
        {
            _service = service;
        }

        // GET: api/Notification
        [HttpGet]
        public async Task<IActionResult> GetMyNotifications()
        {
            int userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var notifications =
                await _service.GetMyNotificationsAsync(userId);

            return Ok(notifications);
        }


        // PUT: api/Notification/5/read
        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            int userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            await _service.MarkAsReadAsync(id, userId);

            return Ok(new
            {
                Success = true,
                Message = "Notification marked as read."
            });
        }
    }
}