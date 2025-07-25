using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamingApplication.Commands.Authentication
{
    public class ForgotPasswordCommand
    {
        public string Email { get; set; }
        public string Otp { get; set; }
        public string Password { get; set; }
    }
}
