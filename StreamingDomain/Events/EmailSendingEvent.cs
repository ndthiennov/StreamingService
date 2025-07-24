using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamingDomain.Events
{
    public class EmailSendingEvent
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public bool IsOtp { get; set; }
        public string DeviceId { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public string Os { get; set; }
        public string Browser { get; set; }
        public string MessageId { get; set; }
    }
}
