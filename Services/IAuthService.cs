using DentalCareAPI.DTOs;

namespace DentalCareAPI.Services
{
    public interface IAuthService
    {
        Task<AdminResponseDto> CreateAdminAsync(AdminDto adminDto);
        Task<LoginResponseDto> LoginAsync(LoginDto loginDto, string ipAddress);
        Task<RefreshTokenResponseDto> RefreshTokenAsync(string token, string ipAddresh);
        Task<bool> RevokeTokenAsync(string token, string ipAddress);
        Task<(bool Status, string Message)> UpdatePasswordAsync( int userId,ChangePasswordDto dto);
        Task<ProfileDto?> ProfileAsync(int userId);
    }
}
 