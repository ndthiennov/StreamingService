using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamingShared.Helpers.Interfaces
{
    public interface IDataEncryptionHelper
    {
        string Hash(string password);
        bool Verify(string hashedPassword, string plainPassword);
    }
}
