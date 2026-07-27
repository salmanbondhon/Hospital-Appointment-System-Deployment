using AutoMapper;
using HospitalAPI.DTOs;
using HospitalAPI.Exceptions;
using HospitalAPI.Interfaces;
using HospitalAPI.Models;

namespace HospitalAPI.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _repository;
        private readonly IMapper _mapper;

        public DepartmentService(IDepartmentRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DepartmentDto>> GetAllAsync()
        {
            var departments = await _repository.GetAllAsync();

            return _mapper.Map<IEnumerable<DepartmentDto>>(departments);
           
        }

        public async Task<DepartmentDto?> GetByIdAsync(int id)
        {
            var department = await _repository.GetByIdAsync(id);

            if (department == null)
                return null;

            return _mapper.Map<DepartmentDto>(department);
        }

        public async Task<DepartmentDto> AddAsync(CreateDepartmentDto dto)
        {
            var department = _mapper.Map<Department>(dto);

            await _repository.AddAsync(department);
            await _repository.SaveChangesAsync();

            return _mapper.Map<DepartmentDto>(department);
        }

        public async Task UpdateAsync(int id, UpdateDepartmentDto dto)
        {
            var department = await _repository.GetByIdAsync(id);

            if (department == null)
            {
                throw new BusinessException("Department not found.");
            }

            _mapper.Map(dto, department);

            await _repository.UpdateAsync(department);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var department = await _repository.GetByIdAsync(id);

            if (department == null)
            {
                throw new BusinessException("Department not found.");
            }

            await _repository.DeleteAsync(department);
            await _repository.SaveChangesAsync();
        }
    }
}
