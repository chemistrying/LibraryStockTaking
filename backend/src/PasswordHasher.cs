using System.Security.Cryptography;
using System.Text;

namespace LibrarySystemApi;

public static class PasswordHasher
{
    public static string HashPassword(string password, byte[] salt) 
    {
        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
        byte[] saltedPassword = new byte[passwordBytes.Length + salt.Length];

        // Concatenate password and salt
        Buffer.BlockCopy(passwordBytes, 0, saltedPassword, 0, passwordBytes.Length);
        Buffer.BlockCopy(salt, 0, saltedPassword, passwordBytes.Length, salt.Length);

        // Hash the concatenated password and salt
        byte[] hashedBytes = SHA256.HashData(saltedPassword);

        // Concatenate the salt and hashed password for storage
        byte[] hashedPasswordWithSalt = new byte[hashedBytes.Length + salt.Length];
        Buffer.BlockCopy(salt, 0, hashedPasswordWithSalt, 0, salt.Length);
        Buffer.BlockCopy(hashedBytes, 0, hashedPasswordWithSalt, salt.Length, hashedBytes.Length);

        return Convert.ToBase64String(hashedPasswordWithSalt);
    }

    public static byte[] GenerateSalt()
    {
        byte[] salt = new byte[16];
        RandomNumberGenerator.Fill(salt);
        return salt;
    }

    public static byte[] GetSaltFromHash(string passwordHash) {
        // Get salt
        byte[] hashedPasswordWithSalt = Convert.FromBase64String(passwordHash);
        byte[] salt = new byte[16];
        Buffer.BlockCopy(hashedPasswordWithSalt, 0, salt, 0, salt.Length);

        return salt;
    }
}