using Microsoft.Extensions.DependencyInjection;
using StreamingApplication.Commands.Handlers.Authentication;
using StreamingApplication.Interfaces.Commands.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamingApplication
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ILoginCommandHandler, LoginCommandHandler>();
            services.AddScoped<IVerifyDeviceCommandHandler, VerifyDeviceCommandHandler>();
            services.AddScoped<ISubmitUserEmailHandler, SubmitUserEmailHandler>();
            services.AddScoped<IInviteUserCommandHandler, InviteUserCommandHandler>();
            services.AddScoped<ISendOtpCommandHandler, SendOtpCommandHandler>();
            services.AddScoped<IForgotPasswordCommandHandler, ForgotPasswordCommandHandler>();

            return services;
        }
    }
}
