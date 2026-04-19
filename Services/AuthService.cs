using RetailOrdering.API.Common.Enums;
using RetailOrdering.API.Data;
using RetailOrdering.API.DTOs.Auth;
using RetailOrdering.API.Helpers;
using RetailOrdering.API.Models;
using RetailOrdering.API.Repositories.Interfaces;
using RetailOrdering.API.Services.Interfaces;

namespace RetailOrdering.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly AppDbContext _context;
        private readonly JwtHelper _jwtHelper;          // ← ADD THIS

        public AuthService(
            IUserRepository userRepo,
            AppDbContext context,
            JwtHelper jwtHelper)                         // ← INJECT HERE
        {
            _userRepo = userRepo;
            _context = context;
            _jwtHelper = jwtHelper;                      // ← ASSIGN HERE
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            if (await _userRepo.ExistsByEmailAsync(dto.Email))
                throw new ArgumentException("Email is already registered.");

            var user = new User
            {
                FullName = dto.FullName.Trim(),
                Email = dto.Email.ToLower().Trim(),
                PasswordHash = PasswordHelper.HashPassword(dto.Password),
                PhoneNumber = dto.Phone.Trim(),
                Role = UserRole.Customer,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var createdUser = await _userRepo.AddAsync(user);

            var loyalty = new LoyaltyAccount
            {
                UserId = createdUser.UserId,
                Points = 0,
            };
            _context.LoyaltyAccounts.Add(loyalty);
            await _context.SaveChangesAsync();

            return GenerateAuthResponse(createdUser);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _userRepo.GetByEmailAsync(dto.Email)
                ?? throw new UnauthorizedAccessException("Invalid email or password.");

            if (!user.IsActive)
                throw new UnauthorizedAccessException("Account is deactivated. Contact support.");

            if (!PasswordHelper.VerifyPassword(dto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid email or password.");

            await _userRepo.UpdateAsync(user);

            return GenerateAuthResponse(user);
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
        {
            var userId = _jwtHelper.GetUserIdFromToken(refreshToken);       // ← USE INSTANCE

            var user = await _userRepo.GetByIdAsync(userId)
                ?? throw new UnauthorizedAccessException("Invalid refresh token.");

            if (!user.IsActive)
                throw new UnauthorizedAccessException("Account is deactivated.");

            return GenerateAuthResponse(user);
        }

        public async Task RevokeTokenAsync(string refreshToken)
        {
            await Task.CompletedTask;
        }

        public async Task<int?> GetUserIdFromTokenAsync(string token)
        {
            return await Task.FromResult(_jwtHelper.GetUserIdFromToken(token));  // ← USE INSTANCE
        }

        private AuthResponseDto GenerateAuthResponse(User user)
        {
            var token = _jwtHelper.GenerateToken(user);                          // ← USE INSTANCE
            var refreshToken = _jwtHelper.GenerateRefreshToken(user);            // ← USE INSTANCE

            return new AuthResponseDto
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.ToString(),
                Token = token,
                RefreshToken = refreshToken,
                TokenExpiry = DateTime.UtcNow.AddHours(_jwtHelper.TokenExpirationHours)  // ← USE INSTANCE
            };
        }
    }
}