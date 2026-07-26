using HospitalAPI.DTOs;

namespace HospitalAPI.Interfaces
{
    public interface IDepartmentService
    {
        Task<IEnumerable<DepartmentDto>> GetAllAsync();

        Task<DepartmentDto?> GetByIdAsync(int id);

        Task<DepartmentDto> AddAsync(CreateDepartmentDto dto);

        Task<bool> UpdateAsync(int id, UpdateDepartmentDto dto);

        Task<bool> DeleteAsync(int id);
    
}
}
