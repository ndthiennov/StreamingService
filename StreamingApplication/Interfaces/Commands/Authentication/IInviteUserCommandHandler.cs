using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamingApplication.Interfaces.Commands.Authentication
{
    public interface IInviteUserCommandHandler
    {
        Task<bool> InviteUser(string email);
    }
}
