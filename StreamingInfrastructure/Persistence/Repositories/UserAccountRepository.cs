using Dapper;
using StreamingDomain.Entities;
using StreamingDomain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamingInfrastructure.Persistence.Repositories
{
    public class UserAccountRepository : IUserAccountRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        public UserAccountRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<UserAccount?> GetUserByEmail(string email)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"
                SELECT 
                    ua.*, 
                    fp.Id AS fpId, fp.DeviceId, fp.IpAddress, fp.UserAgent, fp.Os, fp.Browser
                FROM UserAccount ua
                LEFT JOIN FingerPrint fp ON ua.id = fp.UserAccountId
                WHERE ua.email = @email";

            var user = await connection.QueryAsync<UserAccount, FingerPrint, UserAccount>(sql, 
                (user, fingerprint) =>
                {
                    if (fingerprint != null)
                    {
                        user.FingerPrints = new List<FingerPrint>() { fingerprint };
                    }
                    else
                    {
                        user.FingerPrints = new List<FingerPrint>();
                    }

                    return user;
                }, new {email = email}, splitOn: "fpId");

            var userWithFirst = user.FirstOrDefault();

            if (userWithFirst != null)
            {
                userWithFirst.FingerPrints = user
                    .Where(u => u.FingerPrints != null && u.FingerPrints.Count > 0)
                    .SelectMany(u => u.FingerPrints)
                    .ToList();
            }

            return userWithFirst;
        }
    }
}
