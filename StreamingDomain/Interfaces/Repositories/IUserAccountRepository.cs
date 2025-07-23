using StreamingDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamingDomain.Interfaces.Repositories
{
    public interface IUserAccountRepository
    {
        Task<UserAccount?> GetUserByEmail(string email);
    }
}
