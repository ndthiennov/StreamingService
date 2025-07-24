using MassTransit;
using StreamingDomain.Events;
using StreamingDomain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamingInfrastructure.Messaging.Consumers
{
    public class EmailSendingConsumer : IConsumer<EmailSendingEvent>
    {
        private readonly IMessageLogRepository _messageLogRepository;
        public EmailSendingConsumer(IMessageLogRepository messageLogRepository)
        {
            _messageLogRepository = messageLogRepository;
        }
        public async Task Consume(ConsumeContext<EmailSendingEvent> context)
        {
            var evt = context.Message;

            var message = await _messageLogRepository.GetMessageById(evt.MessageId);
            if(message != null)
            {
                return;
            }

            // Create otp
            var random = new Random();
            var otpBuilder = new StringBuilder();
            for (int i = 0; i < 6; i++)
            {
                otpBuilder.Append(random.Next(0, 10)); // Create a number from 0–9
            }
            string otp = otpBuilder.ToString();


            //await _emailService.SendAsync(evt.Email, evt.Subject, evt.Body);
        }
    }
}
