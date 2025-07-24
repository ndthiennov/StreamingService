using Dapper;
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
        private readonly IDbConnection _connection;
        public TokenRepository(IDbConnection connection)
        {
            _connection = connection;
        }
        public async Task<bool> AddRefreshToken(int userAccountId, string refreshToken)
        {
            var sql = @"
            INSERT INTO Token (GeneratedToken, Type, UserAccountId, ExpiredAt)
            VALUES (@GeneratedToken, @Type, @UserAccountId, @ExpiredAt)";

            await _connection.ExecuteAsync(sql, new
            {
                GeneratedToken = refreshToken,
                Type = "Refresh",
                UserAccountId = userAccountId,
                ExpiredAt = DateTime.UtcNow.AddDays(30)
            });

            return true;
        }
    }
}
