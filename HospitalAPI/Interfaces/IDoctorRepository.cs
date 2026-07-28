using HospitalAPI.Models;

namespace HospitalAPI.Interfaces
{
    public interface IDoctorRepository
    {
        Task<IEnumerable<Doctor>> GetAllAsync();

        Task<Doctor?> GetByIdAsync(int id);

        Task<Doctor?> GetByUserIdAsync(int userId);

        Task AddAsync(Doctor doctor);

        Task UpdateAsync(Doctor doctor);

        Task DeleteAsync(Doctor doctor);

        Task SaveChangesAsync();
    }
}