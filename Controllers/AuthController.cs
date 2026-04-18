using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailOrdering.API.Common;
using RetailOrdering.API.DTOs.Auth;
using RetailOrdering.API.Services.Interfaces;
using System.Security.Claims;

namespace RetailOrdering.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Register a new customer account
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            _logger.LogInformation("Registration attempt for email: {Email}", dto.Email);

            var result = await _authService.RegisterAsync(dto);

            return CreatedAtAction(
                nameof(Register),
                new { id = result.UserId },
                ApiResponse<AuthResponseDto>.SuccessResult(result, "Registration successful."));
        }

        /// <summary>
        /// Login with email and password
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            _logger.LogInformation("Login attempt for email: {Email}", dto.Email);

            var result = await _authService.LoginAsync(dto);

            return Ok(ApiResponse<AuthResponseDto>.SuccessResult(result, "Login successful."));
        }

        /// <summary>
        /// Refresh JWT token
        /// </summary>
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto)
        {
            var result = await _authService.RefreshTokenAsync(dto.RefreshToken);

            return Ok(ApiResponse<AuthResponseDto>.SuccessResult(result, "Token refreshed."));
        }

        /// <summary>
        /// Logout / revoke token
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenDto? dto)
        {
            if (dto != null)
                await _authService.RevokeTokenAsync(dto.RefreshToken);

            return Ok(ApiResponse<object>.SuccessResult(null, "Logged out successfully."));
        }
    }

    // Small DTO placed here for simplicity — can be moved to DTOs/Auth/
    public class RefreshTokenDto
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}