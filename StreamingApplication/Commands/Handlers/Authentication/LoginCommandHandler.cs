using StreamingApplication.Commands.Authentication;
using StreamingApplication.Dtos;
using StreamingApplication.Interfaces.Commands.Authentication;
using StreamingDomain.Entities;
using StreamingDomain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamingApplication.Commands.Handlers.Authentication
{
    public class LoginCommandHandler : ILoginCommandHandler
    {
        private readonly IUserAccountRepository _userAccountRepository;
        public LoginCommandHandler(IUserAccountRepository userAccountRepository)
        {
            _userAccountRepository = userAccountRepository;
        }

        public async Task<ResultDto<string>> UserLogin(LoginCommand loginCommand)
        {
            var user = await _userAccountRepository.GetUserByEmail(loginCommand.Email);

            // Check user existence
            if (user == null)
            {
                return new ResultDto<string>
                {
                    IsSuccess = false,
                    Error = "User not existed",
                    ErrorCode = "400",
                };
            }

            // Check user role
            if(user.Role != "User")
            {
                return new ResultDto<string>
                {
                    IsSuccess = false,
                    Error = "User is not a regular user",
                    ErrorCode = "403",
                };
            }

            // Check user account lock
            if(user.LockoutEnabled || user.LockoutEnd > DateTime.UtcNow)
            {
                return new ResultDto<string>
                {
                    IsSuccess = false,
                    Error = "User account is locked",
                    ErrorCode = "423",
                };
            }

            // Check password
            if (!StreamingShared.Helpers.DataEncryptionHelper.MatchCodeHashHMACSHA512(loginCommand.Password, user.HashedPassword, user.KeyPassword))
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

                return new ResultDto<string>
                {
                    IsSuccess = false,
                    Error = "Password is not corrected",
                    ErrorCode = "400",
                };
            }

            // Check first signed in device
            bool existedDevice = false;
            if (user.FingerPrints != null)
            {
                existedDevice = user.FingerPrints.Any(f =>
                    f.DeviceId == loginCommand.FingerPrint.DeviceId &&
                    f.IpAddress == loginCommand.FingerPrint.IpAddress &&
                    f.UserAgent == loginCommand.FingerPrint.UserAgent &&
                    f.Os == loginCommand.FingerPrint.Os);
            }
            
            if (!existedDevice)
            {

            }

            return new ResultDto<string>
            {
                IsSuccess = true,
                Data = "",
                ErrorCode = "200",
            };
        }
    }
}
