using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RetailOrdering.API.Common;
using RetailOrdering.API.DTOs.Auth;
using RetailOrdering.API.Services.Interfaces;

namespace RetailOrdering.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Get current user's profile
        /// </summary>
        [HttpGet("profile")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> GetProfile()
        {
            var userId = int.Parse(User.FindFirst("userId")?.Value
                ?? throw new UnauthorizedAccessException("Invalid token."));

            _logger.LogInformation("Fetching profile for user: {UserId}", userId);

            var user = await _userService.GetProfileAsync(userId);

            var profileData = new
            {
                user.UserId,
                user.FullName,
                user.Email,
                user.PhoneNumber,
                user.Address,
                user.Role,
                user.IsActive,
                user.CreatedAt
            };

            return Ok(ApiResponse<object>.SuccessResult(profileData));
        }

        /// <summary>
        /// Update current user's profile
        /// </summary>
        [HttpPut("profile")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            var userId = int.Parse(User.FindFirst("userId")?.Value
                ?? throw new UnauthorizedAccessException("Invalid token."));

            _logger.LogInformation("Updating profile for user: {UserId}", userId);

            var user = await _userService.UpdateProfileAsync(userId, dto);

            var profileData = new
            {
                user.UserId,
                user.FullName,
                user.Email,
                user.PhoneNumber,
                user.Address
            };

            return Ok(ApiResponse<object>.SuccessResult(profileData, "Profile updated successfully."));
        }

        /// <summary>
        /// Change current user's password
        /// </summary>
        [HttpPut("change-password")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userId = int.Parse(User.FindFirst("userId")?.Value
                ?? throw new UnauthorizedAccessException("Invalid token."));

            await _userService.ChangePasswordAsync(userId, dto);

            return Ok(ApiResponse<object>.SuccessResult(null, "Password changed successfully."));
        }

        /// <summary>
        /// Get all users (Admin only)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> GetAllUsers()
        {
            _logger.LogInformation("Admin fetching all users");

            var users = await _userService.GetAllUsersAsync();

            var usersData = users.Select(u => new
            {
                u.UserId,
                u.FullName,
                u.Email,
                u.PhoneNumber,
                u.Role,
                u.IsActive,
                u.CreatedAt
            });

            return Ok(ApiResponse<object>.SuccessResult(usersData));
        }

        /// <summary>
        /// Get user by ID (Admin only)
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
                return NotFound(ApiResponse<object>.Fail("User not found."));

            var userData = new
            {
                user.UserId,
                user.FullName,
                user.Email,
                user.PhoneNumber,
                user.Address,
                user.Role,
                user.IsActive,
                user.CreatedAt
            };

            return Ok(ApiResponse<object>.SuccessResult(userData));
        }
    }
}
