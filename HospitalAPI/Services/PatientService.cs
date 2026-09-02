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


        // =================================================
        // GET ALL
        // =================================================

        public async Task<IEnumerable<PatientDto>> GetAllAsync()
        {
            var patients =
                await _repository.GetAllAsync();

            return _mapper.Map<IEnumerable<PatientDto>>(
                patients);
        }


        // =================================================
        // GET BY ID
        // =================================================

        public async Task<PatientDto?> GetByIdAsync(int id)
        {
            var patient =
                await _repository.GetByIdAsync(id);

            if (patient == null)
            {
                return null;
            }

            return _mapper.Map<PatientDto>(patient);
        }


        // =================================================
        // GET CURRENT LOGGED-IN PATIENT
        // =================================================

        public async Task<PatientDto?> GetMyProfileAsync(
            int userId)
        {
            var patient =
                await _repository.GetByUserIdAsync(userId);

            if (patient == null)
            {
                return null;
            }

            return _mapper.Map<PatientDto>(patient);
        }


        // =================================================
        // CREATE PATIENT
        // =================================================

        public async Task<PatientDto> AddAsync(
            CreatePatientDto dto,
            int currentUserId,
            string currentUserRole)
        {
            // =================================================
            // PATIENT CREATES OWN PROFILE
            // =================================================

            if (currentUserRole == UserRole.Patient.ToString())
            {
                var user =
                    await _userRepository.GetByIdAsync(
                        currentUserId);

                if (user == null)
                {
                    throw new BusinessException(
                        "Patient user account not found.");
                }


                var existingPatient =
                    await _repository.GetByUserIdAsync(
                        currentUserId);

                if (existingPatient != null)
                {
                    throw new BusinessException(
                        "Patient profile already exists.");
                }


                var patient = new Patient
                {
                    UserId = user.Id,

                    FullName = dto.FullName,

                    Age = dto.Age,

                    Gender = dto.Gender,

                    PhoneNumber = dto.PhoneNumber,

                    Address = dto.Address,

                    BloodGroup = dto.BloodGroup
                };


                await _repository.AddAsync(patient);

                await _repository.SaveChangesAsync();


                return _mapper.Map<PatientDto>(
                    patient);
            }


            // =================================================
            // ADMIN CREATES PATIENT
            // =================================================

            if (currentUserRole == UserRole.Admin.ToString())
            {
                // Check email
                var existingUser =
                    await _userRepository.GetByEmailAsync(
                        dto.Email);

                if (existingUser != null)
                {
                    throw new BusinessException(
                        "Email already exists.");
                }


                // Validate password
                if (string.IsNullOrWhiteSpace(dto.Password))
                {
                    throw new BusinessException(
                        "Password is required.");
                }


                // =================================================
                // CREATE USER
                // =================================================

                var user = new User
                {
                    FullName = dto.FullName,

                    Email = dto.Email,

                    PasswordHash =
                        BCrypt.Net.BCrypt.HashPassword(
                            dto.Password),

                    Role = UserRole.Patient
                };


                await _userRepository.AddAsync(user);

                await _userRepository.SaveChangesAsync();


                // =================================================
                // CREATE PATIENT PROFILE
                // =================================================

                var patient = new Patient
                {
                    UserId = user.Id,

                    FullName = dto.FullName,

                    Age = dto.Age,

                    Gender = dto.Gender,

                    PhoneNumber = dto.PhoneNumber,

                    Address = dto.Address,

                    BloodGroup = dto.BloodGroup
                };


                await _repository.AddAsync(patient);

                await _repository.SaveChangesAsync();


                return _mapper.Map<PatientDto>(
                    patient);
            }


            throw new BusinessException(
                "You are not authorized to create a patient.");
        }


        // =================================================
        // UPDATE PATIENT
        // =================================================

        public async Task UpdateAsync(
            int id,
            UpdatePatientDto dto,
            int currentUserId,
            string currentUserRole)
        {
            var patient =
                await _repository.GetByIdAsync(id);

            if (patient == null)
            {
                throw new BusinessException(
                    "Patient not found.");
            }


            // =================================================
            // PATIENT CAN ONLY UPDATE OWN PROFILE
            // =================================================

            if (currentUserRole ==
                UserRole.Patient.ToString())
            {
                if (patient.UserId != currentUserId)
                {
                    throw new BusinessException(
                        "You can only update your own profile.");
                }
            }


            // =================================================
            // UPDATE PATIENT PROFILE
            // =================================================

            patient.FullName =
                dto.FullName;

            patient.Age =
                dto.Age;

            patient.Gender =
                dto.Gender;

            patient.PhoneNumber =
                dto.PhoneNumber;

            patient.Address =
                dto.Address;

            patient.BloodGroup =
                dto.BloodGroup;


            // =================================================
            // GET USER ACCOUNT
            // =================================================

            var user =
                await _userRepository.GetByIdAsync(
                    patient.UserId);

            if (user == null)
            {
                throw new BusinessException(
                    "Patient user account not found.");
            }


            // =================================================
            // UPDATE USER NAME
            // =================================================

            user.FullName =
                dto.FullName;


            // =================================================
            // UPDATE EMAIL
            // =================================================

            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                var existingUser =
                    await _userRepository.GetByEmailAsync(
                        dto.Email);

                if (existingUser != null &&
                    existingUser.Id != user.Id)
                {
                    throw new BusinessException(
                        "Email already exists.");
                }

                user.Email =
                    dto.Email;
            }


            // =================================================
            // UPDATE PASSWORD
            // =================================================

           /* if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                user.PasswordHash =
                    BCrypt.Net.BCrypt.HashPassword(
                        dto.Password);
            }*/


            // =================================================
            // SAVE
            // =================================================

            await _repository.UpdateAsync(
                patient);

            await _repository.SaveChangesAsync();

            await _userRepository.SaveChangesAsync();
        }


        // =================================================
        // DELETE
        // =================================================

        public async Task DeleteAsync(int id)
        {
            var patient =
                await _repository.GetByIdAsync(id);

            if (patient == null)
            {
                throw new BusinessException(
                    "Patient not found.");
            }


            await _repository.DeleteAsync(
                patient);

            await _repository.SaveChangesAsync();
        }
    }
}