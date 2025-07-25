using Microsoft.Extensions.Logging;
using StreamingApplication.Interfaces.Commands.Authentication;
using StreamingDomain.Caches;
using StreamingDomain.Interfaces.Caches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamingApplication.Commands.Handlers.Authentication
{
    public class SubmitUserEmailHandler : ISubmitUserEmailHandler
    {
        private readonly ILogger<SubmitUserEmailHandler> _logger;
        private readonly IAccountInstance _accountInstance;
        public SubmitUserEmailHandler(ILogger<SubmitUserEmailHandler> logger,
            IAccountInstance accountInstance)
        {
            _logger = logger;
            _accountInstance = accountInstance;
        }
        public async Task<bool> SubmitUserEmail(string email)
        {
            try
            {
                var account = new AccountCache()
                {
                    Email = email,
                    Token = null,
                    TokenEnd = null,
                    Otp = null,
                    OtpEnd = null,
                };

                await _accountInstance.Add(account);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Application-Commands-Handlers-Authentication-SubmitUserEmail");
                //Console.WriteLine($"Application-Commands-Handlers-Authentication-SubmitUserEmail: {ex.Message}");
                return false;
            }
        }
    }
}
