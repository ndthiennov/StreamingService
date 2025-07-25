using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
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
        //private readonly IConnectionMultiplexer _redis;
        private readonly IDistributedCache _cache;
        public AccountInstance(IDistributedCache cache
            ) //IDistributedCache cache, IConnectionMultiplexer redis
        {
            //_redis = redis;
            _cache = cache;
        }
        public async Task<AccountCache?> Get(string email) //finger print
        {
            try
            {
                string? accountJson = await _cache.GetStringAsync(email);
                if(string.IsNullOrEmpty(accountJson))
                {
                    return null;
                }
                else
                {
                    var account = JsonSerializer.Deserialize<AccountCache>(accountJson);

                    return account;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Infrastructure-Caches-AccountInstance-Get: {ex.Message}");
                return null;
            }
        }
        public async Task<bool> Add(AccountCache accountCache) //finger print
        {
            try
            {
                //Cache data with datetime expiration
                var options = new DistributedCacheEntryOptions
               {
                   AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30), // expired automatically after 30 minutes
                    SlidingExpiration = TimeSpan.FromMinutes(15) // remove automatically after 15 minutes if not accessed
                };

                var serialized = JsonSerializer.Serialize(accountCache);
                await _cache.SetStringAsync(accountCache.Email, serialized, options);

                return true;

                //var db = _redis.GetDatabase();
                //var serialized = JsonSerializer.Serialize(accountCache);

                //await db.StringSetAsync(accountCache.Email, serialized, TimeSpan.FromMinutes(30));

                //return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Infrastructure-Caches-AccountInstance-Add: {ex.Message}");
                return false;
            }
        }
    }
}
