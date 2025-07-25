using Dapper;
using Microsoft.Extensions.Logging;
using StreamingDomain.Entities;
using StreamingDomain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamingInfrastructure.Persistence.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly ILogger<TokenRepository> _logger;
        private readonly IDbConnectionFactory _connectionFactory;
        public TokenRepository(ILogger<TokenRepository> logger,
            IDbConnectionFactory connectionFactory)
        {
            _logger = logger;
            _connectionFactory = connectionFactory;
        }
        public async Task<bool> AddRefreshToken(int userAccountId, string refreshToken)
        {
            try
            {
                var sql = @"
                    INSERT INTO Token (GeneratedToken, Type, UserAccountId, ExpiredAt)
                    VALUES (@GeneratedToken, @Type, @UserAccountId, @ExpiredAt)";

                using var connection = _connectionFactory.CreateConnection();

                await connection.ExecuteAsync(sql, new
                {
                    GeneratedToken = refreshToken,
                    Type = "Refresh",
                    UserAccountId = userAccountId,
                    ExpiredAt = DateTime.UtcNow.AddDays(30)
                });

                return true;
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                _logger.LogError(ex.Message, "Infrastructure-Repository-TokenRepository-AddRefreshToken");
                //Console.WriteLine($"Infrastructure-Repository-TokenRepository-AddRefreshToken: {ex.Message}");
                return false;
            }
        }
    }
}
