using Microsoft.Extensions.Logging;
using StreamingApplication.Commands.Authentication;
using StreamingApplication.Dtos;
using StreamingApplication.Interfaces.Commands.Authentication;
using StreamingDomain.Entities;
using StreamingDomain.Events;
using StreamingDomain.Interfaces.Commons;
using StreamingDomain.Interfaces.Messaging;
using StreamingDomain.Interfaces.Repositories;
using StreamingShared.Helpers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamingApplication.Commands.Handlers.Authentication
{
    public class LoginCommandHandler : ILoginCommandHandler
    {
        private readonly ILogger<LoginCommandHandler> _logger;
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly ITokenRepository _tokenRepository;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IEventPublisher _eventPublisher;
        private readonly IDataEncryptionHelper _dataEncryptionHelper;
        public LoginCommandHandler(ILogger<LoginCommandHandler> logger,
            IUserAccountRepository userAccountRepository, 
            ITokenGenerator tokenGenerator, 
            IEventPublisher eventPublisher,
            ITokenRepository tokenRepository,
            IDataEncryptionHelper dataEncryptionHelper)
        {
            _logger = logger;
            _userAccountRepository = userAccountRepository;
            _tokenRepository = tokenRepository;
            _tokenGenerator = tokenGenerator;
            _eventPublisher = eventPublisher;
            _dataEncryptionHelper = dataEncryptionHelper;

        }

        public async Task<ResultDto<List<string>>> Login(LoginCommand loginCommand)
        {
            try
            {
                var user = await _userAccountRepository.GetUserByEmail(loginCommand.Email);

                // Check user existence
                if (user == null)
                {
                    return new ResultDto<List<string>>
                    {
                        IsSuccess = false,
                        Error = "User not existed",
                        ErrorCode = "400",
                    };
                }

                //// Check user role
                //if (user.Role != "User")
                //{
                //    return new ResultDto<List<string>>
                //    {
                //        IsSuccess = false,
                //        Error = "User is not a regular user",
                //        ErrorCode = "403",
                //    };
                //}

                // Check user account lock
                if (user.LockoutEnabled || user.LockoutEnd > DateTime.UtcNow)
                {
                    return new ResultDto<List<string>>
                    {
                        IsSuccess = false,
                        Error = "User account is locked",
                        ErrorCode = "400",
                    };
                }

                // Check password
                if (!_dataEncryptionHelper.Verify(user.HashedPassword, loginCommand.Password))
                {
                    // Check access failed number
                    if (user.AccessFailedCount >= 2)
                    {
                        await _userAccountRepository.UpdateUserAccessFailedNumberAndLockout(loginCommand.Email, 0, DateTimeOffset.UtcNow.AddHours(6));
                    }
                    else
                    {
                        await _userAccountRepository.UpdateUserAccessFailedNumberAndLockout(loginCommand.Email, user.AccessFailedCount + 1, DateTimeOffset.UtcNow);
                    }

                    return new ResultDto<List<string>>
                    {
                        IsSuccess = false,
                        Error = "Password is not corrected",
                        ErrorCode = "400",
                    };
                }

                // Check first signed in device
                //bool existedDevice = false;
                //if (user.FingerPrints != null)
                //{
                //    existedDevice = user.FingerPrints.Any(f =>
                //        f.DeviceId == loginCommand.FingerPrint.DeviceId &&
                //        f.IpAddress == loginCommand.FingerPrint.IpAddress &&
                //        f.UserAgent == loginCommand.FingerPrint.UserAgent &&
                //        f.Os == loginCommand.FingerPrint.Os);
                //}

                //if (!existedDevice)
                //{
                //    var messageId = _tokenGenerator.GenerateMessageId();

                //    var evt = new EmailSendingEvent
                //    {
                //        Email = user.Email,
                //        IsOtp = true,
                //        DeviceId = loginCommand.FingerPrint.DeviceId,
                //        IpAddress = loginCommand.FingerPrint.IpAddress,
                //        UserAgent = loginCommand.FingerPrint.UserAgent,
                //        Os = loginCommand.FingerPrint.Os,
                //        MessageId = messageId
                //    };

                //    await _eventPublisher.PublishAsync(evt);
                //    return new ResultDto<List<string>>
                //    {
                //        IsSuccess = true,
                //        Error = "First signed in device, please check your email for OTP"
                //    };
                //}

                string refreshToken = _tokenGenerator.GenerateToken(user.Email);
                string jwtToken = _tokenGenerator.GenerateJwtToken(user.Id, user.Email, user.Role);

                var tokenList = new List<string>
                {
                    jwtToken,
                    refreshToken
                };

                await _tokenRepository.AddRefreshToken(user.Id, refreshToken);

                return new ResultDto<List<string>>
                {
                    IsSuccess = true,
                    Data = tokenList,
                    ErrorCode = "200",
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Application-Commands-Handlers-Authentication-LoginCommandHandler");
                //Console.WriteLine($"Application-Commands-Handlers-Authentication-LoginCommandHandler: {ex.Message}");
                return new ResultDto<List<string>>
                {
                    IsSuccess = false,
                    Error = "An error occurred while processing your request",
                    ErrorCode = "500",
                };
            }
        }
    }
}
