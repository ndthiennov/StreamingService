using Dapper;
using MassTransit;
using Npgsql;
using StreamingDomain.Caches;
using StreamingDomain.Entities;
using StreamingDomain.Events;
using StreamingDomain.Interfaces.Caches;
using StreamingDomain.Interfaces.Commons;
using StreamingDomain.Interfaces.Repositories;
using StreamingInfrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace StreamingInfrastructure.Messaging.Consumers
{
    public class EmailSendingConsumer : IConsumer<EmailSendingEvent>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IMessageLogRepository _messageLogRepository;
        private readonly IAccountInstance _accountInstance;
        private readonly IEmailService _emailService;
        private readonly ITokenGenerator _tokenGenerator;

        public EmailSendingConsumer(IDbConnectionFactory connectionFactory, 
            IMessageLogRepository messageLogRepository, 
            IAccountInstance accountInstance, 
            IEmailService emailService,
            ITokenGenerator tokenGenerator)
        {
            _connectionFactory = connectionFactory;
            _messageLogRepository = messageLogRepository;
            _accountInstance = accountInstance;
            _emailService = emailService;
            _tokenGenerator = tokenGenerator;
        }
        public async Task Consume(ConsumeContext<EmailSendingEvent> context)
        {
            var evt = context.Message;

            var message = await _messageLogRepository.GetMessageById(evt.MessageId);
            if(message != null)
            {
                return;
            }

            if (evt.IsOtp)
            {
                // Create otp
                var random = new Random();
                var otp = new StringBuilder();
                for (int i = 0; i < 6; i++)
                {
                    otp.Append(random.Next(0, 10)); // Create a number from 0–9
                }

                var account = new AccountCache()
                {
                    Email = evt.Email,
                    Token = null,
                    TokenEnd = null,
                    Otp = otp.ToString(),
                    OtpEnd = DateTime.UtcNow.AddMinutes(30)
                };

                await _accountInstance.Add(account);

                var log = new MessagingLog()
                {
                    Id = evt.MessageId,
                    Description = "OTP verification",
                    CreatedAt = DateTime.UtcNow
                };

                using var connection = (NpgsqlConnection)_connectionFactory.CreateConnection();
                await connection.OpenAsync();

                using var transaction = connection.BeginTransaction();

                try
                {
                    const string sql = @"
                        INSERT INTO MessagingLog (Id, Description, CreatedAt)
                        VALUES (@Id, @Description, @CreatedAt);";

                    await connection.ExecuteAsync(sql, log, transaction);

                    await _emailService.SendEmailAsync(evt.Email, "Verification OTP", account.Otp);

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            else
            {
                // Create otp
                var token = _tokenGenerator.GenerateToken(evt.Email);

                var account = new AccountCache()
                {
                    Email = evt.Email,
                    Token = token,
                    TokenEnd = DateTime.UtcNow.AddDays(3),
                    Otp = null,
                    OtpEnd = null
                };

                await _accountInstance.Add(account);

                var log = new MessagingLog()
                {
                    Id = evt.MessageId,
                    Description = "Token verification",
                    CreatedAt = DateTime.UtcNow
                };

                using var connection = (NpgsqlConnection)_connectionFactory.CreateConnection();
                await connection.OpenAsync();

                using var transaction = connection.BeginTransaction();

                try
                {
                    const string sql = @"
                        INSERT INTO MessagingLog (Id, Description, CreatedAt)
                        VALUES (@Id, @Description, @CreatedAt);";

                    await connection.ExecuteAsync(sql, log, transaction);

                    await _emailService.SendEmailAsync(evt.Email, "Verification Token", token);

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }

            


            //await _emailService.SendAsync(evt.Email, evt.Subject, evt.Body);
        }
    }
}
