using BAMF_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BAMF_API.Interfaces.AdminInterfaces
{
    public interface IAdminService
    {
        Task<IEnumerable<Admin>> GetAllAdminsAsync();
        Task<Admin?> GetAdminByIdAsync(int id);
        Task CreateAdminAsync(string username, string password);
        Task UpdateAdminPasswordAsync(int id, string currentPassword, string newPassword);
        Task DeleteAdminAsync(int id, string confirmPassword);
    }
}

// M.B