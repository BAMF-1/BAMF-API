using BAMF_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BAMF_API.Interfaces.UserInterfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task UpdateAsync(User user);
        Task DeleteAsync(User user);
        Task SaveChangesAsync();
    }
}

// M.B