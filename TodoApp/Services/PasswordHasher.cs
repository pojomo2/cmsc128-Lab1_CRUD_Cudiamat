using BCrypt.Net;

public static class PasswordHasher
{
    public static string Hash(string plainTextPassword)
    => BCrypt.Net.BCrypt.HashPassword(plainTextPassword);

    public static bool Verify(string plainTextPassword, string storedHash)
    => BCrypt.Net.BCrypt.Verify(plainTextPassword, storedHash);
    
}