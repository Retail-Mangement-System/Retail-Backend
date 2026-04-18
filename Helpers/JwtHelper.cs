using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using RetailOrdering.API.Models;

namespace RetailOrdering.API.Helpers;

public class JwtHelper
{
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expiryInHours;

    public int TokenExpirationHours => _expiryInHours;

    public JwtHelper(Microsoft.Extensions.Configuration.IConfiguration config)
    {
        var jwtSection = config.GetSection("JwtSettings");

        _secretKey = jwtSection["SecretKey"]
            ?? throw new InvalidOperationException("JwtSettings:SecretKey is missing from configuration.");

        _issuer = jwtSection["Issuer"] ?? "RetailOrderingAPI";
        _audience = jwtSection["Audience"] ?? "RetailOrderingClient";
        _expiryInHours = int.Parse(jwtSection["ExpiryInHours"] ?? "24");
    }

    /// <summary>
    /// Generates a JWT access token for the given user.
    /// </summary>
    public string GenerateToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString()),
            // Custom claim so we can easily read userId in controllers
            new("userId", user.UserId.ToString()),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_expiryInHours),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Generates a long-lived refresh token (also a JWT but with 7-day expiry).
    /// </summary>
    public string GenerateRefreshToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new("userId", user.UserId.ToString()),
            new("refreshToken", "true"),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Extracts the userId from any valid token (access or refresh).
    /// Returns null if the token is invalid.
    /// </summary>
    public int GetUserIdFromToken(string token)
    {
        try
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var handler = new JwtSecurityTokenHandler();

            // Validate token without checking expiry (for refresh tokens that may be near expiry)
            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _issuer,
                ValidAudience = _audience,
                IssuerSigningKey = key,
                ValidateLifetime = false  // We handle expiry separately
            };

            var principal = handler.ValidateToken(token, parameters, out _);
            var userIdClaim = principal.FindFirst("userId") ?? principal.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
                return userId;

            return 0;
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>
    /// Validates a token and returns the ClaimsPrincipal if valid.
    /// Returns null if invalid or expired.
    /// </summary>
    public ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var handler = new JwtSecurityTokenHandler();

            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidIssuer = _issuer,
                ValidAudience = _audience,
                IssuerSigningKey = key,
                ClockSkew = TimeSpan.Zero
            };

            return handler.ValidateToken(token, parameters, out _);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Checks if a token is expired without throwing.
    /// </summary>
    public bool IsTokenExpired(string token)
    {
        var principal = ValidateToken(token);
        return principal == null;
    }
}