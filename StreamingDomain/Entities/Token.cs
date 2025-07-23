using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamingDomain.Entities
{
    public class Token
    {
        public string GeneratedToken { get; set; }
        public string Type { get; set; }
        public int UserAccouId { get; set; }
        public DateTime ExpiredAt { get; set; }
    }
}
