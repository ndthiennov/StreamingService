using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamingDomain.Interfaces.Repositories
{
    public interface ITokenRepository
    {
        Task<bool> AddRefreshToken(int userAccountId, string refreshToken);
    }
}
