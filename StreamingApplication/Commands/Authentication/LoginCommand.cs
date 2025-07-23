using StreamingApplication.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamingApplication.Commands.Authentication
{
    public class LoginCommand
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public FingerPrintDto FingerPrint { get; set; }
    }
}
