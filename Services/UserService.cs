using BAMF_API.DTOs.Requests.User;
using BAMF_API.Interfaces.UserInterfaces;
using BAMF_API.Models;
using System.Security.Cryptography;
using System.Text;

namespace BAMF_API.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // Admin functions
        public async Task<IEnumerable<User>> GetAllUsersAsync() => await _userRepository.GetAllAsync();

        public async Task<User?> GetUserByIdAsync(int id) => await _userRepository.GetByIdAsync(id);

        public async Task UpdateUserAsync(int id, User updatedUser)
        {
            var existing = await _userRepository.GetByIdAsync(id);
            if (existing == null) throw new Exception("User not found");

            existing.Email = updatedUser.Email;
            await _userRepository.UpdateAsync(existing);
        }

        public async Task DeleteUserAsync(int id)
        {
            var existing = await _userRepository.GetByIdAsync(id);
            if (existing == null) throw new Exception("User not found");
            await _userRepository.DeleteAsync(existing);
        }

        // profile functions
        public async Task<User?> GetOwnProfileAsync(int id) =>
            await _userRepository.GetByIdAsync(id);

        public async Task UpdateOwnProfileAsync(int id, UserUpdateDto dto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) throw new Exception("User not found");

            user.Email = dto.Email;

            if (!string.IsNullOrEmpty(dto.CurrentPassword) && !string.IsNullOrEmpty(dto.NewPassword))
            {
                if (!VerifyPassword(dto.CurrentPassword, user.PasswordHash, user.PasswordSalt))
                    throw new Exception("Incorrect current password");

                CreatePasswordHash(dto.NewPassword, out var newHash, out var newSalt);
                user.PasswordHash = newHash;
                user.PasswordSalt = newSalt;
            }

            await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteOwnAccountAsync(int id, UserDeleteDto dto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) throw new Exception("User not found");

            if (!VerifyPassword(dto.Password, user.PasswordHash, user.PasswordSalt))
                throw new Exception("Incorrect password");

            await _userRepository.DeleteAsync(user);
        }

        // verification and hashing
        private void CreatePasswordHash(string password, out string hash, out string salt)
        {
            using var hmac = new HMACSHA512();
            salt = Convert.ToBase64String(hmac.Key);
            hash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
        }

        private bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            using var hmac = new HMACSHA512(Convert.FromBase64String(storedSalt));
            var computed = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(computed) == storedHash;
        }
    }
}

// M.B