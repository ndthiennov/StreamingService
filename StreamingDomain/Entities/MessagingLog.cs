using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamingDomain.Entities
{
    public class MessagingLog
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
