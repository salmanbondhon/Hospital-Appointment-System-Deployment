using HospitalAPI.DTOs;

namespace HospitalAPI.Interfaces
{
    public interface IPatientService
    {
        Task<IEnumerable<PatientDto>> GetAllAsync();

        Task<PatientDto?> GetByIdAsync(int id);

        Task<PatientDto> AddAsync(
            CreatePatientDto dto,
            int currentUserId,
            string currentUserRole);

        Task UpdateAsync(
            int id,
            UpdatePatientDto dto,
            int currentUserId,
            string currentUserRole);

        Task DeleteAsync(int id);
    }
}