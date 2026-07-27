using HospitalAPI.DTOs;

namespace HospitalAPI.Interfaces
{
    public interface IUserService
    {
        Task RegisterAsync(RegisterDto dto);

        Task<LoginResponseDto> LoginAsync(LoginDto dto);
    }
}