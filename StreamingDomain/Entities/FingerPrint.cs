using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamingDomain.Entities
{
    public class FingerPrint
    {
        public int Id { get; set; }
        public int UserAccountId { get; set; }
        public string DeviceId { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public string Os { get; set; }
        public string Browser { get; set; }
        public UserAccount UserAccount { get; set; }
    }
}
