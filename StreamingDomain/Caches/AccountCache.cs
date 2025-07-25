using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamingDomain.Caches
{
    public class AccountCache
    {
        public string Email { get; set; }
        public string? Token { get; set; }
        public DateTime? TokenEnd { get; set; }
        public string? Otp {  get; set; }
        public DateTime? OtpEnd { get; set; }
    }
}
