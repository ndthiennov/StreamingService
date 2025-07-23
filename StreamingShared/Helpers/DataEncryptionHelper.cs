using StreamingShared.CommonDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace StreamingShared.Helpers
{
    public class DataEncryptionHelper
    {
        public static HashSaltDto HMACSHA512(string code)
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
        }
    }
}
