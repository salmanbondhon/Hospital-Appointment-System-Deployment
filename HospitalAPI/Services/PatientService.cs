using AutoMapper;
using HospitalAPI.DTOs;
using HospitalAPI.Interfaces;
using HospitalAPI.Models;

namespace HospitalAPI.Services
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _repository;
        private readonly IMapper _mapper;

        public PatientService(IPatientRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PatientDto>> GetAllAsync()
        {
            var patients = await _repository.GetAllAsync();

            return _mapper.Map<IEnumerable<PatientDto>>(patients);
        }

        public async Task<PatientDto?> GetByIdAsync(int id)
        {
            var patient = await _repository.GetByIdAsync(id);

            if (patient == null)
                return null;

            return _mapper.Map<PatientDto>(patient);
        }

        public async Task<PatientDto> AddAsync(CreatePatientDto dto)
        {
            var patient = _mapper.Map<Patient>(dto);

            await _repository.AddAsync(patient);
            await _repository.SaveChangesAsync();

            return _mapper.Map<PatientDto>(patient);
        }

        public async Task<bool> UpdateAsync(int id, UpdatePatientDto dto)
        {
            var patient = await _repository.GetByIdAsync(id);

            if (patient == null)
                return false;

            _mapper.Map(dto, patient);

            await _repository.UpdateAsync(patient);
            await _repository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var patient = await _repository.GetByIdAsync(id);

            if (patient == null)
                return false;

            await _repository.DeleteAsync(patient);
            await _repository.SaveChangesAsync();

            return true;
        }
    }
}