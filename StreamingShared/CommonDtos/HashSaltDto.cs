using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamingShared.CommonDtos
{
    public class HashSaltDto
    {
        public byte[] hashedCode { get; set; }
        public byte[] keyCode { get; set; }
    }
}
