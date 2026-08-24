using HospitalAPI.Models;

namespace HospitalAPI.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);

        Task<User?> GetByIdAsync(int id);

        Task AddAsync(User user);

        Task SaveChangesAsync();

        Task<IEnumerable<User>> GetAvailableDoctorUsersAsync();

        Task<IEnumerable<User>> GetAvailablePatientUsersAsync();
    }
}