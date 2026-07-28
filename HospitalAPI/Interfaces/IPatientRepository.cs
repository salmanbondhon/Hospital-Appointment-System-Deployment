using HospitalAPI.Models;

namespace HospitalAPI.Interfaces
{
    public interface IPatientRepository
    {
        Task<IEnumerable<Patient>> GetAllAsync();

        Task<Patient?> GetByIdAsync(int id);

        Task<Patient?> GetByUserIdAsync(int userId);

        Task AddAsync(Patient patient);

        Task UpdateAsync(Patient patient);

        Task DeleteAsync(Patient patient);

        Task SaveChangesAsync();
    }
}