using BAMF_API.Data;
using BAMF_API.Interfaces.AdminInterfaces;
using BAMF_API.Models;
using Microsoft.EntityFrameworkCore;

namespace BAMF_API.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ApplicationDbContext _context;

        public AdminRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Admin>> GetAllAsync() =>
            await _context.Admins.ToListAsync();

        public async Task<Admin?> GetByIdAsync(int id) =>
            await _context.Admins.FindAsync(id);

        public async Task<Admin?> GetByUserNameAsync(string username) =>
            await _context.Admins.FirstOrDefaultAsync(a => a.UserName == username);

        public async Task AddAsync(Admin admin)
        {
            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Admin admin)
        {
            _context.Admins.Update(admin);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Admin admin)
        {
            _context.Admins.Remove(admin);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync() =>
            await _context.SaveChangesAsync();
    }
}

// M.B