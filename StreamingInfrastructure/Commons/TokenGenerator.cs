using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StreamingDomain.Interfaces.Commons;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace StreamingInfrastructure.Commons
{
    public class TokenGenerator : ITokenGenerator
    {
        private readonly IConfiguration _config;
        public TokenGenerator(IConfiguration config)
        {
            _config = config;
        }
        public string GenerateMessageId()
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var uniquePart = Guid.NewGuid().ToString("N")[..6]; // first 6 chars
            return $"MSG-{timestamp}-{uniquePart}";
        }
        public string GenerateJwtToken(int userId, string email, string role)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Secret"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                //new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                //new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(type: "id", value: userId.ToString()),
                new Claim(type: "email", value: email),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public string GenerateToken(string email)
        {
            var random = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(random);

            var baseToken = Convert.ToBase64String(random);
            return $"{email}-{baseToken}";
        }
    }
}
