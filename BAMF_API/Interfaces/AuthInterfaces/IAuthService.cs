using BAMF_API.DTOs.Requests.Auth;
using System.Threading.Tasks;

namespace BAMF_API.Interfaces.AuthInterfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
        Task<AuthResponseDto> LoginAdminAsync(AdminLoginDto dto);
    }
}

// M.B