using Microsoft.Extensions.Logging;
using StreamingApplication.Interfaces.Commands.Authentication;
using StreamingDomain.Events;
using StreamingDomain.Interfaces.Commons;
using StreamingDomain.Interfaces.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamingApplication.Commands.Handlers.Authentication
{
    public class SendOtpCommandHandler : ISendOtpCommandHandler
    {
        private readonly ILogger<InviteUserCommandHandler> _logger;
        private readonly IEventPublisher _eventPublisher;
        private readonly ITokenGenerator _tokenGenerator;
        public SendOtpCommandHandler(ILogger<InviteUserCommandHandler> logger,
            IEventPublisher eventPublisher,
            ITokenGenerator tokenGenerator)
        {
            _logger = logger;
            _eventPublisher = eventPublisher;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<bool> SendOtp(string email)
        {
            try
            {
                var messageId = _tokenGenerator.GenerateMessageId();

                var evt = new EmailSendingEvent
                {
                    Email = email,
                    IsOtp = false,
                    DeviceId = null,
                    IpAddress = null,
                    UserAgent = null,
                    Os = null,
                    MessageId = messageId
                };

                await _eventPublisher.PublishAsync(evt);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Application-Commands-Handlers-Authentication-SendOtpCommandHandler");
                //Console.WriteLine($"Application-Commands-Handlers-Authentication-InviteUserCommandHandler: {ex.Message}");
                return false;
            }
        }
    }
}
