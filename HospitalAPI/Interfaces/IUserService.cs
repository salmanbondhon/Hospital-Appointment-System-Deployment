using HospitalAPI.DTOs;

namespace HospitalAPI.Interfaces
{
    public interface IUserService
    {
        Task RegisterAsync(RegisterDto dto);

        Task<LoginResponseDto> LoginAsync(LoginDto dto);

        Task<IEnumerable<UserDto>> GetAvailableDoctorUsersAsync();
        Task<IEnumerable<UserDto>> GetAvailablePatientUsersAsync();

        Task ForgotPasswordAsync(ForgotPasswordDto dto);

        Task ResetPasswordAsync(ResetPasswordDto dto);

        Task ChangePasswordAsync(
    int userId,
    ChangePasswordDto dto);
    }
}