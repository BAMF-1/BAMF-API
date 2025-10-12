using BAMF_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BAMF_API.Interfaces.AdminInterfaces
{
    public interface IAdminRepository
    {
        Task<IEnumerable<Admin>> GetAllAsync();
        Task<Admin?> GetByIdAsync(int id);
        Task<Admin?> GetByUserNameAsync(string username);
        Task AddAsync(Admin admin);
        Task UpdateAsync(Admin admin);
        Task DeleteAsync(Admin admin);
        Task SaveChangesAsync();
    }
}

// M.B