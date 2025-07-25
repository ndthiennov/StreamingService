using StreamingApplication.Commands.Authentication;
using StreamingApplication.Dtos;
using StreamingDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamingApplication.Interfaces.Commands.Authentication
{
    public interface ILoginCommandHandler
    {
        Task<ResultDto<List<string>>> Login(LoginCommand loginCommand);
    }
}
