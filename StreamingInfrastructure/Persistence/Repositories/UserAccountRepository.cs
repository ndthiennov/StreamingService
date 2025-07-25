using Dapper;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<UserAccountRepository> _logger;
        private readonly IDbConnectionFactory _connectionFactory;
        public UserAccountRepository(ILogger<UserAccountRepository> logger,
            IDbConnectionFactory connectionFactory)
        {
            _logger = logger;
            _connectionFactory = connectionFactory;
        }

        public async Task<UserAccount?> GetUserByEmail(string email)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var sql = @"
                        SELECT 
                            ua.*, 
                            fp.Id AS fpId, fp.DeviceId, fp.IpAddress, fp.UserAgent, fp.Os, fp.Browser
                        FROM UserAccount ua
                        LEFT JOIN FingerPrint fp ON ua.id = fp.UserAccountId
                        WHERE ua.email = @Email";

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
                    }, new { Email = email }, splitOn: "fpId");

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
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                _logger.LogError(ex.Message, "Infrastructure-UserAccountRepository-GetUserByEmail");
                //Console.WriteLine($"Infrastructure-UserAccountRepository-GetUserByEmail: {ex.Message}");
                return null;
            }
        }
        public async Task<bool> UpdateUserAccessFailedNumberAndLockout(string email, int failedCount, DateTimeOffset? lockoutEnd)
        {
            try
            {
                string sql = @"
                UPDATE UserAccount
                SET AccessFailedCount = @AccessFailedCount,
                    LockoutEnd = @LockoutEnd
                WHERE Email = @Email;";
                using var connection = _connectionFactory.CreateConnection();
                await connection.ExecuteAsync(sql, new
                {
                    Email = email,
                    AccessFailedCount = failedCount,
                    LockoutEnd = lockoutEnd
                });

                return true;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                _logger.LogError(ex.Message, "Infrastructure-Repository-UserAccountRepository-UpdateUserAccessFailedNumberAndLockout");
                //Console.WriteLine($"Infrastructure-Repository-UserAccountRepository-UpdateUserAccessFailedNumberAndLockout: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> UpdateUserPassword(string email, string hashedPassword)
        {
            try
            {
                string sql = @"
                UPDATE UserAccount
                SET HashedPassword = @HashedPassword
                WHERE Email = @Email;";
                using var connection = _connectionFactory.CreateConnection();
                await connection.ExecuteAsync(sql, new
                {
                    Email = email,
                    HashedPassword = hashedPassword
                });

                return true;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                _logger.LogError(ex.Message, "Infrastructure-Repository-UserAccountRepository-UpdateUserPassword");
                //Console.WriteLine($"Infrastructure-Repository-UserAccountRepository-UpdateUserPassword: {ex.Message}");
                return false;
            }
        }
    }
}
