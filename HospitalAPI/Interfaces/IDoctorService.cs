using HospitalAPI.DTOs;

namespace HospitalAPI.Interfaces
{
    public interface IDoctorService
    {
        Task<IEnumerable<DoctorDto>> GetAllAsync();

        Task<DoctorDto?> GetByIdAsync(int id);

        Task<DoctorDto?> GetMyProfileAsync(int userId);


        Task<DoctorDto> AddAsync(
            CreateDoctorDto dto);

        Task UpdateAsync(
            int id,
            UpdateDoctorDto dto);

        Task DeleteAsync(
            int id);
    }
}