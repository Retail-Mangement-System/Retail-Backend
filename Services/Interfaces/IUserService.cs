<<<<<<< HEAD
﻿using RetailOrdering.API.DTOs.User;

namespace RetailOrdering.API.Services;

public interface IUserService
{
    Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
    Task<UserResponseDto?> GetUserByIdAsync(int id);
    Task<UserResponseDto> CreateUserAsync(CreateUserDto dto);
    Task<UserResponseDto?> UpdateUserAsync(int id, UpdateUserDto dto);
    Task<bool> DeleteUserAsync(int id);
}
=======
﻿using RetailOrdering.API.DTOs.Auth;
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
>>>>>>> origin/dev
