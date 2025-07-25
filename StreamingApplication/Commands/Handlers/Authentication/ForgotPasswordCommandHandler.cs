using Microsoft.Extensions.Logging;
using StreamingApplication.Commands.Authentication;
using StreamingApplication.Dtos;
using StreamingApplication.Interfaces.Commands.Authentication;
using StreamingDomain.Interfaces.Caches;
using StreamingDomain.Interfaces.Repositories;
using StreamingShared.Helpers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamingApplication.Commands.Handlers.Authentication
{
    public class ForgotPasswordCommandHandler : IForgotPasswordCommandHandler
    {
        private readonly ILogger<ForgotPasswordCommandHandler> _logger;
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly IAccountInstance _accountInstance;
        private readonly IDataEncryptionHelper _dataEncryptionHelper;
        public ForgotPasswordCommandHandler(ILogger<ForgotPasswordCommandHandler> logger,
            IUserAccountRepository userAccountRepository,
            IAccountInstance accountInstance,
            IDataEncryptionHelper dataEncryptionHelper)
        {
            _logger = logger;
            _userAccountRepository = userAccountRepository;
            _accountInstance = accountInstance;
            _dataEncryptionHelper = dataEncryptionHelper;
        }
        public async Task<ResultDto<bool>> ForgotPassword(ForgotPasswordCommand forgotPasswordCommand)
        {
            try
            {
                var user = await _userAccountRepository.GetUserByEmail(forgotPasswordCommand.Email);

                // Check user existence
                if (user == null)
                {
                    return new ResultDto<bool>
                    {
                        IsSuccess = false,
                        Error = "User not existed",
                        ErrorCode = "400",
                    };
                }

                // Check otp from cache and otp from user input
                var accountCache = await _accountInstance.Get(forgotPasswordCommand.Email);
                if (accountCache == null)
                {
                    return new ResultDto<bool>
                    {
                        IsSuccess = false,
                        Error = "Otp is not matched",
                        ErrorCode = "400",
                    };
                }

                if(accountCache.Otp != forgotPasswordCommand.Otp)
                {
                    return new ResultDto<bool>
                    {
                        IsSuccess = false,
                        Error = "Otp is not matched",
                        ErrorCode = "400",
                    };
                }

                // Update user password
                var hashedPassword = _dataEncryptionHelper.Hash(forgotPasswordCommand.Password);
                await _userAccountRepository.UpdateUserPassword(forgotPasswordCommand.Email, hashedPassword);

                return new ResultDto<bool>
                {
                    IsSuccess = true,
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Application-Commands-Handlers-Authentication-ForgotPasswordCommandHandler");
                return new ResultDto<bool>
                {
                    IsSuccess = false,
                    Error = ex.Message,
                    ErrorCode = "500",
                };
            }
            
        }
    }
}
