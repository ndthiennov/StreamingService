using Microsoft.Extensions.DependencyInjection;
using StreamingShared.Helpers;
using StreamingShared.Helpers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamingShared
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddShared(this IServiceCollection services)
        {
            services.AddTransient<IDataEncryptionHelper, DataEncryptionHelper>();

            return services;
        }
    }
}
