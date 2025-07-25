using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
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
    public class MessageLogRepository : IMessageLogRepository
    {
        private readonly ILogger<MessageLogRepository> _logger;
        private readonly IDbConnectionFactory _connectionFactory;
        public MessageLogRepository(ILogger<MessageLogRepository> logger,
            IDbConnectionFactory connectionFactory)
        {
            _logger = logger;
            _connectionFactory = connectionFactory;
        }

        public async Task<MessagingLog?> GetMessageById(string id)
        {
            try
            {
                var sql = "SELECT Id, Description, CreatedAt FROM MessagingLog WHERE Id = @Id";

                using var connection = _connectionFactory.CreateConnection();

                var result = await connection.QuerySingleOrDefaultAsync<MessagingLog>(sql, new { Id = id });

                return result;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                _logger.LogError(ex.Message, "Infrastructure-Repository-MessageLogRepository-GetMessageById");
                //Console.WriteLine($"Infrastructure-Repository-MessageLogRepository-GetMessageById: {ex.Message}");
                return null;
            }
            
        }
    }
}
