using Dapper;
using MassTransit;
using Microsoft.Extensions.Logging;
using Npgsql;
using StreamingDomain.Caches;
using StreamingDomain.Entities;
using StreamingDomain.Events;
using StreamingDomain.Interfaces.Caches;
using StreamingDomain.Interfaces.Commons;
using StreamingDomain.Interfaces.Repositories;
using StreamingInfrastructure.Persistence;
using StreamingInfrastructure.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using static System.Net.WebRequestMethods;

namespace StreamingInfrastructure.Messaging.Consumers
{
    public class EmailSendingConsumer : IConsumer<EmailSendingEvent>
    {
        private readonly ILogger<EmailSendingConsumer> _logger;
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IMessageLogRepository _messageLogRepository;
        private readonly IAccountInstance _accountInstance;
        private readonly IEmailService _emailService;
        private readonly ITokenGenerator _tokenGenerator;

        public EmailSendingConsumer(ILogger<EmailSendingConsumer> logger,
            IDbConnectionFactory connectionFactory, 
            IMessageLogRepository messageLogRepository, 
            IAccountInstance accountInstance, 
            IEmailService emailService,
            ITokenGenerator tokenGenerator)
        {
            _logger = logger;
            _connectionFactory = connectionFactory;
            _messageLogRepository = messageLogRepository;
            _accountInstance = accountInstance;
            _emailService = emailService;
            _tokenGenerator = tokenGenerator;
        }
        public async Task Consume(ConsumeContext<EmailSendingEvent> context)
        {
            try
            {
                var evt = context.Message;

                var message = await _messageLogRepository.GetMessageById(evt.MessageId);
                if (message != null)
                {
                    return;
                }

                /*var accountFromCache = await _accountInstance.Get(evt.Email);
                var account = new AccountCache();*/

                if (evt.IsOtp)
                {
                    // Create otp
                    var random = new Random();
                    var otpBuilder = new StringBuilder();
                    for (int i = 0; i < 6; i++)
                    {
                        otpBuilder.Append(random.Next(0, 10)); // Create a number from 0–9
                    }

                    string otp = otpBuilder.ToString();

                    /*if (accountFromCache == null)
                    {
                        account = new AccountCache()
                        {
                            Email = evt.Email,
                            Token = null,
                            TokenEnd = null,
                            Otp = otp,
                            OtpEnd = DateTime.UtcNow.AddMinutes(30)
                        };
                    }
                    else
                    {
                        account = new AccountCache()
                        {
                            Email = evt.Email,
                            Token = accountFromCache.Token,
                            TokenEnd = accountFromCache.TokenEnd,
                            Otp = otp,
                            OtpEnd = DateTime.UtcNow.AddMinutes(30)
                        };
                    }

                    await _accountInstance.Add(account);*/

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

                        string body = $"<div style=\"font-family: Helvetica,Arial,sans-serif;min-width:1000px;overflow:auto;line-height:2\">\r\n  <div style=\"margin:50px auto;width:70%;padding:20px 0\">\r\n    <div style=\"border-bottom:1px solid #eee\">\r\n      <a href=\"\" style=\"font-size:1.4em;color: #00466a;text-decoration:none;font-weight:600\">Streaming</a>\r\n    </div>\r\n    <p style=\"font-size:1.1em\">Hi,</p>\r\n    <p>Thank you for choosing Streaming. Use the following OTP to complete your Sign Up procedures. OTP is valid for 15 minutes</p>\r\n    <h2 style=\"background: #00466a;margin: 0 auto;width: max-content;padding: 0 10px;color: #fff;border-radius: 4px;\">{otp}</h2>\r\n    <p style=\"font-size:0.9em;\">Regards,<br />Streaming</p>\r\n    <hr style=\"border:none;border-top:1px solid #eee\" />\r\n    <div style=\"float:right;padding:8px 0;color:#aaa;font-size:0.8em;line-height:1;font-weight:300\">\r\n      <p>Streaming Inc</p>\r\n      <p>1600 Amphitheatre Parkway</p>\r\n      <p>Vietnam</p>\r\n    </div>\r\n  </div>\r\n</div>";

                        await _emailService.SendEmailAsync(evt.Email, "Verification OTP", body);

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        // Log the exception or handle it as needed
                        _logger.LogError(ex.Message, "Infrastructure-Messaging-EmailSendingConsumer");
                        //Console.WriteLine($"Infrastructure-Messaging-EmailSendingConsumer: {ex.Message}");
                        throw;
                    }
                }
                else
                {
                    // Create token
                    var token = _tokenGenerator.GenerateToken(evt.Email);

                    //if (accountFromCache == null)
                    //{
                    //    account = new AccountCache()
                    //    {
                    //        Email = evt.Email,
                    //        Token = token,
                    //        TokenEnd = DateTime.UtcNow.AddMinutes(30),
                    //        Otp = null,
                    //        OtpEnd = null
                    //    };
                    //}
                    //else
                    //{
                    //    account = new AccountCache()
                    //    {
                    //        Email = evt.Email,
                    //        Token = token,
                    //        TokenEnd = DateTime.UtcNow.AddMinutes(30),
                    //        Otp = accountFromCache.Otp,
                    //        OtpEnd = accountFromCache.OtpEnd
                    //    };
                    //}

                    //await _accountInstance.Add(account);

                    var messageLog = new MessagingLog()
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

                        await connection.ExecuteAsync(sql, messageLog, transaction);

                        string tokenLink = $"https://localhost:4200/verify?token={token}";
                        string body = $"<div style=\"font-family: Helvetica,Arial,sans-serif;min-width:1000px;overflow:auto;line-height:2\">\r\n  <div style=\"margin:50px auto;width:70%;padding:20px 0\">\r\n    <div style=\"border-bottom:1px solid #eee\">\r\n      <a href=\"\" style=\"font-size:1.4em;color: #00466a;text-decoration:none;font-weight:600\">Streaming</a>\r\n    </div>\r\n    <p style=\"font-size:1.1em\">Hi,</p>\r\n    <p>Thank you for choosing Streaming. Use the following OTP to complete your Sign Up procedures. OTP is valid for 15 minutes</p>\r\n   <a href=\"{tokenLink}\"> <h2 style=\"background: #00466a;margin: 0 auto;width: max-content;padding: 0 10px;color: #fff;border-radius: 4px;\">{tokenLink}</h2> </a> \r\n    <p style=\"font-size:0.9em;\">Regards,<br />Streaming</p>\r\n    <hr style=\"border:none;border-top:1px solid #eee\" />\r\n    <div style=\"float:right;padding:8px 0;color:#aaa;font-size:0.8em;line-height:1;font-weight:300\">\r\n      <p>Streaming Inc</p>\r\n      <p>1600 Amphitheatre Parkway</p>\r\n      <p>Vietnam</p>\r\n    </div>\r\n  </div>\r\n</div>";

                        await _emailService.SendEmailAsync(evt.Email, "Invite joining our Streaming", body);

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        // Log the exception or handle it as needed
                        _logger.LogError(ex.Message, "Infrastructure-Messaging-EmailSendingConsumer");
                        //Console.WriteLine($"Infrastructure-Messaging-EmailSendingConsumer: {ex.Message}");
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                _logger.LogError(ex.Message, "Infrastructure-Messaging-EmailSendingConsumer");
                //Console.WriteLine($"Infrastructure-Messaging-EmailSendingConsumer: {ex.Message}");
                throw;
            }
        }
    }
}
