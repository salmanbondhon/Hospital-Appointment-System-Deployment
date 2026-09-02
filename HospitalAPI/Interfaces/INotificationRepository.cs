using HospitalAPI.Models;

namespace HospitalAPI.Interfaces
{
    public interface INotificationRepository
    {
        Task<IEnumerable<Notification>> GetByUserIdAsync(int userId);

        Task<Notification?> GetByIdAsync(int id);

        Task AddAsync(Notification notification);

        Task UpdateAsync(Notification notification);

        Task SaveChangesAsync();
    }
}