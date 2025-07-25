using Microsoft.Extensions.Caching.Distributed;
using StreamingDomain.Caches;
using StreamingDomain.Interfaces.Caches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace StreamingInfrastructure.Caches
{
    public class AccountInstance : IAccountInstance
    {
        private readonly IDistributedCache _cache;
        public AccountInstance(IDistributedCache cache)
        {
            _cache = cache;
        }
        public async Task<bool> Add(AccountCache accountCache) //finger print
        {
            // Cache lại dữ liệu với thời gian hết hạn
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30), // Tự hết hạn sau 10 phút
                SlidingExpiration = TimeSpan.FromMinutes(15) // Nếu không truy cập sau 2 phút thì cũng xoá
            };

            var serialized = JsonSerializer.Serialize(accountCache);
            await _cache.SetStringAsync(accountCache.Email, serialized, options);

            return true;
        }
    }
}
