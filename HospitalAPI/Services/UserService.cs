using BCrypt.Net;
using HospitalAPI.DTOs;
using HospitalAPI.Exceptions;
using HospitalAPI.Interfaces;
using HospitalAPI.Models;
using HospitalAPI.Configurations;
using HospitalAPI.Enums;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HospitalAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IPatientRepository _patientRepository;
        private readonly JwtSettings _jwtSettings;

        public UserService(
     IUserRepository repository,
     IPatientRepository patientRepository,
     IOptions<JwtSettings> jwtSettings)
        {
            _repository = repository;
            _patientRepository = patientRepository;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task RegisterAsync(RegisterDto dto)
        {
            // Check if email already exists
            var existingUser =
                await _repository.GetByEmailAsync(dto.Email);

            if (existingUser != null)
            {
                throw new BusinessException(
                    "Email already exists.");
            }


            // =========================
            // CREATE USER
            // =========================

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,

                PasswordHash =
                    BCrypt.Net.BCrypt.HashPassword(dto.Password),

                // Public registration = Patient
                Role = UserRole.Patient
            };


            await _repository.AddAsync(user);

            await _repository.SaveChangesAsync();


            // =========================
            // CREATE PATIENT PROFILE
            // =========================

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


            await _patientRepository.AddAsync(patient);

            await _patientRepository.SaveChangesAsync();
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _repository.GetByEmailAsync(dto.Email);

            if (user == null)
            {
                throw new BusinessException("Invalid email or password.");
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(
                dto.Password,
                user.PasswordHash);

            if (!isPasswordValid)
            {
                throw new BusinessException("Invalid email or password.");
            }

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.FullName),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role.ToString())
    };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.Key));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddMinutes(
                _jwtSettings.DurationInMinutes);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: credentials);

            return new LoginResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expires = expires
            };
        }


        // =========================
        // GET AVAILABLE DOCTOR USERS
        // =========================

        public async Task<IEnumerable<UserDto>> GetAvailableDoctorUsersAsync()
        {
            var users = await _repository.GetAvailableDoctorUsersAsync();

            return users.Select(u => new UserDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email
            });
        }
        // =========================
        // GET AVAILABLE PATIENT USERS
        // =========================

        public async Task<IEnumerable<UserDto>> GetAvailablePatientUsersAsync()
        {
            var users =
                await _repository.GetAvailablePatientUsersAsync();

            return users.Select(u => new UserDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email
            });
        }
    }



}