using MassTransit;
using StreamingDomain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamingInfrastructure.Messaging.Consumers
{
    public class EmailSendingConsumer : IConsumer<EmailSendingEvent>
    {
        public async Task Consume(ConsumeContext<EmailSendingEvent> context)
        {
            var evt = context.Message;

            //await _emailService.SendAsync(evt.Email, evt.Subject, evt.Body);
        }
    }
}
