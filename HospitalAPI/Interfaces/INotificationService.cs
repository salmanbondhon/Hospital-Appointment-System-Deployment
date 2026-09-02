using HospitalAPI.DTOs;

namespace HospitalAPI.Interfaces
{
    public interface INotificationService
    {
        Task<IEnumerable<NotificationDto>> GetMyNotificationsAsync(int userId);

        Task MarkAsReadAsync(int notificationId, int userId);

        Task CreateNotificationAsync(
            int userId,
            string title,
            string message);
    }
}