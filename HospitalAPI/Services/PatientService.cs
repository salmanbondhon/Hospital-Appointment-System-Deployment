using AutoMapper;
using HospitalAPI.DTOs;
using HospitalAPI.Enums;
using HospitalAPI.Exceptions;
using HospitalAPI.Interfaces;
using HospitalAPI.Models;

namespace HospitalAPI.Services
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _repository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public PatientService(
      IPatientRepository repository,
      IUserRepository userRepository,
      IMapper mapper)
        {
            _repository = repository;
            _userRepository = userRepository;
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
            // Check if user exists
            var user = await _userRepository.GetByIdAsync(dto.UserId);

            if (user == null)
            {
                throw new BusinessException("User not found.");
            }

            // User must have Patient role
            if (user.Role != UserRole.Patient)
            {
                throw new BusinessException("Selected user is not a patient.");
            }

            // Prevent duplicate patient profile
            var existingPatient = await _repository.GetByUserIdAsync(dto.UserId);

            if (existingPatient != null)
            {
                throw new BusinessException("This user already has a patient profile.");
            }

            var patient = _mapper.Map<Patient>(dto);

            await _repository.AddAsync(patient);
            await _repository.SaveChangesAsync();

            return _mapper.Map<PatientDto>(patient);
        }

        public async Task UpdateAsync(int id, UpdatePatientDto dto)
        {
            var patient = await _repository.GetByIdAsync(id);

            if (patient == null)
            {
                throw new BusinessException("Patient not found.");
            }

            _mapper.Map(dto, patient);

            await _repository.UpdateAsync(patient);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var patient = await _repository.GetByIdAsync(id);

            if (patient == null)
            {
                throw new BusinessException("Patient not found.");
            }

            await _repository.DeleteAsync(patient);
            await _repository.SaveChangesAsync();
        }
    }
}