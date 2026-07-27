using HospitalAPI.DTOs;

namespace HospitalAPI.Interfaces
{
    public interface IDoctorService
    {
        Task<IEnumerable<DoctorDto>> GetAllAsync();

        Task<DoctorDto?> GetByIdAsync(int id);

        Task<DoctorDto> AddAsync(CreateDoctorDto dto);

        Task<bool> UpdateAsync(int id, UpdateDoctorDto dto);

        Task<bool> DeleteAsync(int id);
    }
}