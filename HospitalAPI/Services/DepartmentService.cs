using HospitalAPI.DTOs;
using HospitalAPI.Interfaces;
using HospitalAPI.Models;

namespace HospitalAPI.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _repository;

        public DepartmentService(IDepartmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<DepartmentDto>> GetAllAsync()
        {
            var departments = await _repository.GetAllAsync();

            return departments.Select(d => new DepartmentDto
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description
            });
        }

        public async Task<DepartmentDto?> GetByIdAsync(int id)
        {
            var department = await _repository.GetByIdAsync(id);

            if (department == null)
                return null;

            return new DepartmentDto
            {
                Id = department.Id,
                Name = department.Name,
                Description = department.Description
            };
        }

        public async Task<DepartmentDto> AddAsync(CreateDepartmentDto dto)
        {
            var department = new Department
            {
                Name = dto.Name,
                Description = dto.Description
            };

            await _repository.AddAsync(department);
            await _repository.SaveChangesAsync();

            return new DepartmentDto
            {
                Id = department.Id,
                Name = department.Name,
                Description = department.Description
            };
        }

        public async Task<bool> UpdateAsync(int id, UpdateDepartmentDto dto)
        {
            var department = await _repository.GetByIdAsync(id);

            if (department == null)
                return false;

            department.Name = dto.Name;
            department.Description = dto.Description;

            await _repository.UpdateAsync(department);
            await _repository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var department = await _repository.GetByIdAsync(id);

            if (department == null)
                return false;

            await _repository.DeleteAsync(department);
            await _repository.SaveChangesAsync();

            return true;
        }
    }
}
