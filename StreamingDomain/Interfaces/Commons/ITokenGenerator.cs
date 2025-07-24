using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamingDomain.Interfaces.Commons
{
    public interface ITokenGenerator
    {
        string GenerateMessageId();
        string GenerateJwtToken(int userId, string email, string role);
        string GenerateRefreshToken(int userId);
    }
}
