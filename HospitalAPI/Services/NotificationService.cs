using AutoMapper;
using HospitalAPI.DTOs;
using HospitalAPI.Exceptions;
using HospitalAPI.Interfaces;
using HospitalAPI.Models;

namespace HospitalAPI.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;

        public NotificationService(
            INotificationRepository notificationRepository,
            IMapper mapper)
        {
            _notificationRepository = notificationRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<NotificationDto>> GetMyNotificationsAsync(
            int userId)
        {
            var notifications =
                await _notificationRepository.GetByUserIdAsync(userId);

            return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
        }

        public async Task MarkAsReadAsync(
            int notificationId,
            int userId)
        {
            var notification =
                await _notificationRepository.GetByIdAsync(notificationId);

            if (notification == null)
                throw new BusinessException("Notification not found.");

            if (notification.UserId != userId)
                throw new BusinessException(
                    "You are not authorized to access this notification.");

            if (notification.IsRead)
                return;

            notification.IsRead = true;

            await _notificationRepository.UpdateAsync(notification);

            await _notificationRepository.SaveChangesAsync();
        }

        public async Task CreateNotificationAsync(
            int userId,
            string title,
            string message)
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _notificationRepository.AddAsync(notification);

            await _notificationRepository.SaveChangesAsync();
        }
    }
}