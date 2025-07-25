using StreamingDomain.Caches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamingDomain.Interfaces.Caches
{
    public interface IAccountInstance
    {
        Task<AccountCache?> Get(string email);
        Task<bool> Add(AccountCache accountCache);
    }
}
