using HospitalAPI.DTOs;

namespace HospitalAPI.Interfaces
{
    public interface IPatientService
    {
        Task<IEnumerable<PatientDto>> GetAllAsync();

        Task<PatientDto?> GetByIdAsync(int id);

        Task<PatientDto> AddAsync(CreatePatientDto dto);

        Task<bool> UpdateAsync(int id, UpdatePatientDto dto);

        Task<bool> DeleteAsync(int id);
    }
}