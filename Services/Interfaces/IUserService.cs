using RetailOrdering.API.DTOs.Auth;
using RetailOrdering.API.Models;

namespace RetailOrdering.API.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> GetProfileAsync(int userId);
        Task<User> UpdateProfileAsync(int userId, UpdateProfileDto dto);
        Task ChangePasswordAsync(int userId, ChangePasswordDto dto);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
    }
}
