

using HospitalAPI.DTOs;

namespace HospitalAPI.Interfaces
{
    public interface IPatientService
    {
        Task<IEnumerable<PatientDto>> GetAllAsync();

        Task<PatientDto?> GetByIdAsync(int id);

        Task<PatientDto> AddAsync(CreatePatientDto dto);

        Task UpdateAsync(int id, UpdatePatientDto dto);
        Task DeleteAsync(int id);
    }
}