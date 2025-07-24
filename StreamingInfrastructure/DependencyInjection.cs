using Microsoft.Extensions.DependencyInjection;
using StreamingDomain.Interfaces.Commons;
using StreamingDomain.Interfaces.Messaging;
using StreamingDomain.Interfaces.Repositories;
using StreamingInfrastructure.Commons;
using StreamingInfrastructure.Messaging;
using StreamingInfrastructure.Persistence;
using StreamingInfrastructure.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamingInfrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();

            services.AddScoped<IUserAccountRepository, UserAccountRepository>();
            services.AddScoped<ITokenRepository, TokenRepository>();
            services.AddScoped<IEventPublisher, EventPublisher>();

            services.AddTransient<ITokenGenerator, TokenGenerator>();

            return services;
        }
    }
}
