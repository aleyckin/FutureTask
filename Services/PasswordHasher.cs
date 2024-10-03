using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public static class PasswordHasher
    {
        const int keysize = 64;
        const int iterations = 350000;
        private readonly static HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;

        public static string HashPassword(string password, out byte[] salt)
        {
            salt = RandomNumberGenerator.GetBytes(keysize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                iterations,
                hashAlgorithm,
                keysize);

            return Convert.ToHexString(hash);
        }

        public static bool VerifyPassword(string password, string hash, byte[] salt)
        {
            var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, hashAlgorithm, keysize);

            return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(hash));
        }
    }
}
