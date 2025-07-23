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
            if(user.LockoutEnabled && user.LockoutEnd > DateTime.UtcNow)
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
                    
                }
                else
                {
                    
                }

                return new ResultDto<string>
                {
                    IsSuccess = false,
                    Error = "Password is not corrected",
                    ErrorCode = "400",
                };
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
