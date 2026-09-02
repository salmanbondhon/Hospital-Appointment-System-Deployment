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
using System.Security.Cryptography;
using System.Text;

namespace HospitalAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IPatientRepository _patientRepository;
        private readonly JwtSettings _jwtSettings;
        private readonly IEmailService _emailService;

        public UserService(
    IUserRepository repository,
    IPatientRepository patientRepository,
    IOptions<JwtSettings> jwtSettings,
    IEmailService emailService)
        {
            _repository = repository;
            _patientRepository = patientRepository;
            _jwtSettings = jwtSettings.Value;
            _emailService = emailService;
        }

        public async Task RegisterAsync(RegisterDto dto)
        {
            // Check if email already exists
            var email = dto.Email.Trim();

            var existingUser =
                await _repository.GetByEmailAsync(email);

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
                Email = email,

                PasswordHash =
         BCrypt.Net.BCrypt.HashPassword(dto.Password),

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
            var email = dto.Email.Trim();

            var user =
                await _repository.GetByEmailAsync(email);

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



        public async Task ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var user = await _repository.GetByEmailAsync(dto.Email);

            // Do not reveal whether the email exists
            if (user == null)
            {
                return;
            }

            // Generate secure random token
            var tokenBytes = RandomNumberGenerator.GetBytes(32);

            var token = Convert.ToBase64String(tokenBytes);

            user.PasswordResetToken = token;

            // Token valid for 30 minutes
            user.PasswordResetTokenExpiry =
                DateTime.UtcNow.AddMinutes(30);

            await _repository.SaveChangesAsync();

            // For now we will use this Angular URL
            var resetLink =
                $"http://localhost:4200/reset-password?email={Uri.EscapeDataString(user.Email)}&token={Uri.EscapeDataString(token)}";

            var body =
                EmailTemplateService.PasswordReset(
                    user.FullName,
                    resetLink);

            // IMPORTANT:
            // We need your existing email service here.
            await _emailService.SendEmailAsync(
        user.Email,
        "Reset Your Password",
        body);
        }



        public async Task ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user =
                await _repository.GetByEmailAsync(dto.Email);

            if (user == null)
            {
                throw new BusinessException(
                    "Invalid password reset request.");
            }

            if (string.IsNullOrEmpty(user.PasswordResetToken))
            {
                throw new BusinessException(
                    "Invalid password reset token.");
            }

            if (user.PasswordResetToken != dto.Token)
            {
                throw new BusinessException(
                    "Invalid password reset token.");
            }

            if (!user.PasswordResetTokenExpiry.HasValue ||
                user.PasswordResetTokenExpiry.Value < DateTime.UtcNow)
            {
                throw new BusinessException(
                    "Password reset token has expired.");
            }

            user.PasswordHash =
                BCrypt.Net.BCrypt.HashPassword(
                    dto.NewPassword);

            // Invalidate token after successful reset
            user.PasswordResetToken = null;

            user.PasswordResetTokenExpiry = null;

            await _repository.SaveChangesAsync();
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



        // =========================
        // CHANGE PASSWORD
        // =========================

        public async Task ChangePasswordAsync(
            int userId,
            ChangePasswordDto dto)
        {
            // =========================
            // GET USER
            // =========================

            var user =
                await _repository.GetByIdAsync(userId);

            if (user == null)
            {
                throw new BusinessException(
                    "User account not found.");
            }


            // =========================
            // VALIDATE CURRENT PASSWORD
            // =========================

            if (string.IsNullOrWhiteSpace(dto.CurrentPassword))
            {
                throw new BusinessException(
                    "Current password is required.");
            }


            // =========================
            // VALIDATE NEW PASSWORD
            // =========================

            if (string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                throw new BusinessException(
                    "New password is required.");
            }


            // =========================
            // VALIDATE CONFIRM PASSWORD
            // =========================

            if (string.IsNullOrWhiteSpace(dto.ConfirmPassword))
            {
                throw new BusinessException(
                    "Please confirm your new password.");
            }


            // =========================
            // CHECK CURRENT PASSWORD
            // =========================

            bool isCurrentPasswordValid =
                BCrypt.Net.BCrypt.Verify(
                    dto.CurrentPassword,
                    user.PasswordHash);

            if (!isCurrentPasswordValid)
            {
                throw new BusinessException(
                    "Current password is incorrect.");
            }


            // =========================
            // CHECK NEW PASSWORD MATCH
            // =========================

            if (dto.NewPassword != dto.ConfirmPassword)
            {
                throw new BusinessException(
                    "New password and confirm password do not match.");
            }


            // =========================
            // PREVENT SAME PASSWORD
            // =========================

            if (BCrypt.Net.BCrypt.Verify(
                dto.NewPassword,
                user.PasswordHash))
            {
                throw new BusinessException(
                    "New password must be different from current password.");
            }


            // =========================
            // HASH NEW PASSWORD
            // =========================

            user.PasswordHash =
                BCrypt.Net.BCrypt.HashPassword(
                    dto.NewPassword);


            // =========================
            // SAVE
            // =========================

            await _repository.SaveChangesAsync();
        }



    }
}