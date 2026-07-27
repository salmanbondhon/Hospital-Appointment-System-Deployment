using AutoMapper;
using HospitalAPI.DTOs;
using HospitalAPI.Interfaces;
using HospitalAPI.Models;

namespace HospitalAPI.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _repository;
        private readonly IMapper _mapper;

        public DoctorService(IDoctorRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DoctorDto>> GetAllAsync()
        {
            var doctors = await _repository.GetAllAsync();

            return _mapper.Map<IEnumerable<DoctorDto>>(doctors);
        }

        public async Task<DoctorDto?> GetByIdAsync(int id)
        {
            var doctor = await _repository.GetByIdAsync(id);

            if (doctor == null)
                return null;

            return _mapper.Map<DoctorDto>(doctor);
        }

        public async Task<DoctorDto> AddAsync(CreateDoctorDto dto)
        {
            var doctor = _mapper.Map<Doctor>(dto);

            await _repository.AddAsync(doctor);
            await _repository.SaveChangesAsync();

            return _mapper.Map<DoctorDto>(doctor);
        }

        public async Task<bool> UpdateAsync(int id, UpdateDoctorDto dto)
        {
            var doctor = await _repository.GetByIdAsync(id);

            if (doctor == null)
                return false;

            _mapper.Map(dto, doctor);

            await _repository.UpdateAsync(doctor);
            await _repository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var doctor = await _repository.GetByIdAsync(id);

            if (doctor == null)
                return false;

            await _repository.DeleteAsync(doctor);
            await _repository.SaveChangesAsync();

            return true;
        }
    }
}