using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using StreamingDomain.Interfaces.Caches;
using StreamingDomain.Interfaces.Commons;
using StreamingDomain.Interfaces.Messaging;
using StreamingDomain.Interfaces.Repositories;
using StreamingInfrastructure.Caches;
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
            //services.AddStackExchangeRedisCache(options =>
            //{
            //    options.Configuration = "redis-10391.c124.us-central1-1.gce.redns.redis-cloud.com:10391,password=KgZXwc5G5ciVsExx8PTQh3m5HhZ20Bq7,ssl=True,abortConnect=False";
            //    //options.InstanceName = "AccountInstance";
            //});

            //services.AddStackExchangeRedisCache(options =>
            //{
            //    options.Configuration = "redis://red-d21bkqm3jp1c73fm1dkg:gisLsKbbZzYUjlhUdVwDf4EMHI8kUI0r@red-d21bkqm3jp1c73fm1dkg:6379";
            //    //options.InstanceName = "AccountInstance";
            //});

            //services.AddSingleton<IConnectionMultiplexer>(sp =>
            //{
            //    var config = ConfigurationOptions.Parse("redis-10391.c124.us-central1-1.gce.redns.redis-cloud.com:10391,password=KgZXwc5G5ciVsExx8PTQh3m5HhZ20Bq7,ssl=True,abortConnect=False");
            //    return ConnectionMultiplexer.Connect(config);
            //});

            //services.AddStackExchangeRedisCache(options =>
            //{
            //    options.Configuration = "redis-10391.c124.us-central1-1.gce.redns.redis-cloud.com:10391";
            //    options.ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions
            //    {
            //        EndPoints = { "redis-10391.c124.us-central1-1.gce.redns.redis-cloud.com:10391" },
            //        Password = "KgZXwc5G5ciVsExx8PTQh3m5HhZ20Bq7",
            //        Ssl = true,
            //        AbortOnConnectFail = false
            //    };
            //});

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = "redis://red-d21krh7fte5s73fo38cg:6379";
                options.InstanceName = "AccountInstance";
            });

            services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();

            services.AddScoped<IUserAccountRepository, UserAccountRepository>();
            services.AddScoped<ITokenRepository, TokenRepository>();
            services.AddScoped<IMessageLogRepository, MessageLogRepository>();

            services.AddScoped<IEventPublisher, EventPublisher>();

            services.AddScoped<IAccountInstance, AccountInstance>();

            services.AddScoped<IEmailService, EmailService>();
            services.AddTransient<ITokenGenerator, TokenGenerator>();

            return services;
        }
    }
}
