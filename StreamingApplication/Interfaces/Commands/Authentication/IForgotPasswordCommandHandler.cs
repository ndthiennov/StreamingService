using StreamingApplication.Commands.Authentication;
using StreamingApplication.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamingApplication.Interfaces.Commands.Authentication
{
    public interface IForgotPasswordCommandHandler
    {
        Task<ResultDto<bool>> ForgotPassword(ForgotPasswordCommand forgotPasswordCommand);
    }
}
