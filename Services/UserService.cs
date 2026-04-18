using RetailOrdering.API.DTOs.Auth;
using RetailOrdering.API.Helpers;
using RetailOrdering.API.Models;
using RetailOrdering.API.Repositories.Interfaces;
using RetailOrdering.API.Services.Interfaces;

namespace RetailOrdering.API.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;

        public UserService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<User> GetProfileAsync(int userId)
        {
            var user = await _userRepo.GetByIdAsync(userId)
                ?? throw new KeyNotFoundException("User not found.");

            return user;
        }

        public async Task<User> UpdateProfileAsync(int userId, UpdateProfileDto dto)
        {
            var user = await _userRepo.GetByEmailAsync(
                (await _userRepo.GetByIdAsync(userId))!.Email)
                ?? throw new KeyNotFoundException("User not found.");

            // Check email uniqueness if changing email
            if (!string.IsNullOrWhiteSpace(dto.Email) &&
                dto.Email.ToLower().Trim() != user.Email)
            {
                if (await _userRepo.ExistsByEmailAsync(dto.Email))
                    throw new ArgumentException("Email is already taken.");
                user.Email = dto.Email.ToLower().Trim();
            }

            if (!string.IsNullOrWhiteSpace(dto.FullName))
                user.FullName = dto.FullName.Trim();

            if (!string.IsNullOrWhiteSpace(dto.Phone))
                user.PhoneNumber = dto.Phone.Trim();

            if (dto.Address is not null)
                user.Address = dto.Address.Trim();

            return await _userRepo.UpdateAsync(user);
        }

        public async Task ChangePasswordAsync(int userId, ChangePasswordDto dto)
        {
            var user = await _userRepo.GetByEmailAsync(
                (await _userRepo.GetByIdAsync(userId))!.Email)
                ?? throw new KeyNotFoundException("User not found.");

            if (!PasswordHelper.VerifyPassword(dto.CurrentPassword, user.PasswordHash))
                throw new ArgumentException("Current password is incorrect.");

            if (PasswordHelper.VerifyPassword(dto.NewPassword, user.PasswordHash))
                throw new ArgumentException("New password must be different from current password.");

            user.PasswordHash = PasswordHelper.HashPassword(dto.NewPassword);

            await _userRepo.UpdateAsync(user);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepo.GetAllAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _userRepo.GetByIdAsync(id);
        }
    }
}
