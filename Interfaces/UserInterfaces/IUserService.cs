using BAMF_API.DTOs.Requests.User;
using BAMF_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BAMF_API.Interfaces.UserInterfaces
{
    public interface IUserService
    {
        // Admin CRUD
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task UpdateUserAsync(int id, User updatedUser);
        Task DeleteUserAsync(int id);

        // Profile actions
        Task<User?> GetOwnProfileAsync(int id);
        Task UpdateOwnProfileAsync(int id, UserUpdateDto dto);
        Task DeleteOwnAccountAsync(int id, UserDeleteDto dto);
    }
}
