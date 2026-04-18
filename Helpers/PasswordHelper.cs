namespace RetailOrdering.API.Helpers;

public static class PasswordHelper
{
    /// <summary>
    /// Hashes a plain-text password using BCrypt.
    /// </summary>
    public static string HashPassword(string plainPassword)
    {
        if (string.IsNullOrWhiteSpace(plainPassword))
            throw new ArgumentException("Password cannot be empty.");

        // WorkFactor 12 is the recommended default — balances security & speed
        return BCrypt.Net.BCrypt.HashPassword(plainPassword, workFactor: 12);
    }

    /// <summary>
    /// Verifies a plain-text password against a BCrypt hash.
    /// </summary>
    public static bool VerifyPassword(string plainPassword, string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(plainPassword) || string.IsNullOrWhiteSpace(hashedPassword))
            return false;

        return BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);
    }
}
