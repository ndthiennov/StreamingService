using Microsoft.AspNetCore.Identity;
using StreamingShared.Helpers.Interfaces;

namespace StreamingShared.Helpers
{
    public class DataEncryptionHelper : IDataEncryptionHelper
    {
        private readonly PasswordHasher<object> _hasher = new();

        public string Hash(string password)
        {
            return _hasher.HashPassword(null, password);
        }

        public bool Verify(string hashedPassword, string plainPassword)
        {
            var result = _hasher.VerifyHashedPassword(null, hashedPassword, plainPassword);
            if(result == PasswordVerificationResult.Failed)
            {
                return false;
            }
            return true;
        }
        /*public static HashSaltDto HMACSHA512(string code)
        {
            byte[] codeHash, codeKey;
            using (var hmac = new HMACSHA512())
            {
                codeKey = hmac.Key;
                codeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(code));
            }
            return new HashSaltDto
            {
                hashedCode = codeHash,
                keyCode = codeKey
            };
        }
        public static bool MatchCodeHashHMACSHA512(string code, byte[] hashedCode, byte[] keyCode)
        {
            using (var hmac = new HMACSHA512(keyCode))
            {
                var codeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(code));
                for (int i = 0; i < codeHash.Length; i++)
                {
                    if (codeHash[i] != hashedCode[i])
                    {
                        return false;
                    }
                }
                return true;
            }
        }*/

        /*public static string Argon2Hasher(string password)
        {
            // Create random salt
            byte[] salt = RandomNumberGenerator.GetBytes(16); // 16 bytes = 128 bits

            // Use Argon2id to hash
            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                DegreeOfParallelism = 8, // Number of threads
                MemorySize = 65536,      // 64 MB
                Iterations = 4
            };

            byte[] hash = argon2.GetBytes(32); // Length of hash: 32 bytes = 256 bits

            // Combine salt + hash lại thành 1 chuỗi base64
            string result = Convert.ToBase64String(salt) + "." + Convert.ToBase64String(hash);
            return result;
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            // Tách salt và hash từ chuỗi lưu
            var parts = hashedPassword.Split('.');
            if (parts.Length != 2)
                return false;

            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] storedHash = Convert.FromBase64String(parts[1]);

            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                DegreeOfParallelism = 8,
                MemorySize = 65536,
                Iterations = 4
            };

            byte[] computedHash = argon2.GetBytes(32);

            return storedHash.SequenceEqual(computedHash);
        }*/
    }
}
