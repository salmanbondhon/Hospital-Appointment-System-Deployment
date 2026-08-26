using HospitalAPI.DTOs;
using HospitalAPI.Enums;
using HospitalAPI.Exceptions;
using HospitalAPI.Interfaces;
using HospitalAPI.Models;

namespace HospitalAPI.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _repository;
        private readonly IUserRepository _userRepository;

        public DoctorService(
            IDoctorRepository repository,
            IUserRepository userRepository)
        {
            _repository = repository;
            _userRepository = userRepository;
        }


        // =====================================================
        // GET ALL
        // =====================================================

        public async Task<IEnumerable<DoctorDto>> GetAllAsync()
        {
            var doctors =
                await _repository.GetAllAsync();

            return doctors.Select(d => new DoctorDto
            {
                Id = d.Id,

                FullName = d.FullName,

                Email = d.User?.Email ?? string.Empty,

                Specialization = d.Specialization,

                Qualification = d.Qualification,

                Experience = d.Experience,

                ConsultationFee = d.ConsultationFee,

                AvailableFrom = d.AvailableFrom,

                AvailableTo = d.AvailableTo,

                DepartmentId = d.DepartmentId,

                DepartmentName =
                    d.Department?.Name ?? string.Empty,

                UserId = d.UserId
            });
        }


        // =====================================================
        // GET BY ID
        // =====================================================

        public async Task<DoctorDto?> GetByIdAsync(int id)
        {
            var doctor =
                await _repository.GetByIdAsync(id);

            if (doctor == null)
            {
                return null;
            }

            return new DoctorDto
            {
                Id = doctor.Id,

                FullName = doctor.FullName,

                Email = doctor.User?.Email ?? string.Empty,

                Specialization =
                    doctor.Specialization,

                Qualification =
                    doctor.Qualification,

                Experience =
                    doctor.Experience,

                ConsultationFee =
                    doctor.ConsultationFee,

                AvailableFrom =
                    doctor.AvailableFrom,

                AvailableTo =
                    doctor.AvailableTo,

                DepartmentId =
                    doctor.DepartmentId,

                DepartmentName =
                    doctor.Department?.Name ?? string.Empty,

                UserId =
                    doctor.UserId
            };
        }


        // =====================================================
        // CREATE DOCTOR
        // =====================================================

        public async Task<DoctorDto> AddAsync(
            CreateDoctorDto dto)
        {
            // =================================================
            // CHECK EMAIL
            // =================================================

            var existingUser =
                await _userRepository
                    .GetByEmailAsync(dto.Email);

            if (existingUser != null)
            {
                throw new BusinessException(
                    "Email already exists.");
            }


            // =================================================
            // CREATE USER ACCOUNT
            // =================================================

            var user = new User
            {
                FullName = dto.FullName,

                Email = dto.Email,

                PasswordHash =
                    BCrypt.Net.BCrypt.HashPassword(
                        dto.Password),

                Role = UserRole.Doctor
            };

            await _userRepository.AddAsync(user);

            await _userRepository.SaveChangesAsync();


            // =================================================
            // CREATE DOCTOR PROFILE
            // =================================================

            var doctor = new Doctor
            {
                UserId = user.Id,

                FullName = dto.FullName,

                Specialization =
                    dto.Specialization,

                Qualification =
                    dto.Qualification,

                Experience =
                    dto.Experience,

                ConsultationFee =
                    dto.ConsultationFee,

                AvailableFrom =
                    dto.AvailableFrom,

                AvailableTo =
                    dto.AvailableTo,

                DepartmentId =
                    dto.DepartmentId
            };


            await _repository.AddAsync(doctor);

            await _repository.SaveChangesAsync();


            // =================================================
            // RETURN DTO
            // =================================================

            return new DoctorDto
            {
                Id = doctor.Id,

                FullName = doctor.FullName,

                Email = user.Email,

                Specialization =
                    doctor.Specialization,

                Qualification =
                    doctor.Qualification,

                Experience =
                    doctor.Experience,

                ConsultationFee =
                    doctor.ConsultationFee,

                AvailableFrom =
                    doctor.AvailableFrom,

                AvailableTo =
                    doctor.AvailableTo,

                DepartmentId =
                    doctor.DepartmentId,

                UserId =
                    doctor.UserId
            };
        }


        // =====================================================
        // UPDATE DOCTOR
        // =====================================================

        public async Task UpdateAsync(
            int id,
            UpdateDoctorDto dto)
        {
            // =================================================
            // GET DOCTOR
            // =================================================

            var doctor =
                await _repository.GetByIdAsync(id);

            if (doctor == null)
            {
                throw new BusinessException(
                    "Doctor not found.");
            }


            // =================================================
            // GET USER ACCOUNT
            // =================================================

            var user =
                await _userRepository
                    .GetByIdAsync(doctor.UserId);

            if (user == null)
            {
                throw new BusinessException(
                    "Doctor user account not found.");
            }


            // =================================================
            // VALIDATE EMAIL
            // =================================================

            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                throw new BusinessException(
                    "Email is required.");
            }


            // =================================================
            // CHECK DUPLICATE EMAIL
            // =================================================

            var existingUser =
                await _userRepository
                    .GetByEmailAsync(dto.Email);

            if (existingUser != null &&
                existingUser.Id != user.Id)
            {
                throw new BusinessException(
                    "Email already exists.");
            }


            // =================================================
            // UPDATE DOCTOR PROFILE
            // =================================================

            doctor.FullName =
                dto.FullName;

            doctor.Specialization =
                dto.Specialization;

            doctor.Qualification =
                dto.Qualification;

            doctor.Experience =
                dto.Experience;

            doctor.ConsultationFee =
                dto.ConsultationFee;

            doctor.AvailableFrom =
                dto.AvailableFrom;

            doctor.AvailableTo =
                dto.AvailableTo;

            doctor.DepartmentId =
                dto.DepartmentId;


            // =================================================
            // UPDATE USER
            // =================================================

            user.FullName =
                dto.FullName;

            user.Email =
                dto.Email;


            // =================================================
            // UPDATE PASSWORD
            // =================================================

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                user.PasswordHash =
                    BCrypt.Net.BCrypt.HashPassword(
                        dto.Password);
            }


            // =================================================
            // SAVE CHANGES
            // =================================================

            await _repository.SaveChangesAsync();
        }


        // =====================================================
        // DELETE
        // =====================================================

        public async Task DeleteAsync(int id)
        {
            var doctor =
                await _repository.GetByIdAsync(id);

            if (doctor == null)
            {
                throw new BusinessException(
                    "Doctor not found.");
            }

            await _repository.DeleteAsync(doctor);

            await _repository.SaveChangesAsync();
        }
    }
}