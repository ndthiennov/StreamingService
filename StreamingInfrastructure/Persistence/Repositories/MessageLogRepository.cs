using Dapper;
using Microsoft.Extensions.Configuration;
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
        private readonly IDbConnectionFactory _connectionFactory;
        public MessageLogRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<MessagingLog?> GetMessageById(string id)
        {
            var sql = "SELECT Id, Description, CreatedAt FROM MessagingLog WHERE Id = @Id";

            using var connection = _connectionFactory.CreateConnection();

            var result = await connection.QuerySingleOrDefaultAsync<MessagingLog>(sql, new { Id = id });

            return result;
        }
    }
}
