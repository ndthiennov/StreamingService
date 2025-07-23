using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamingDomain.Entities
{
    public class UserAccount
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public byte[] HashedPassword { get; set; }
        public byte[] KeyPassword { get; set; }
        public bool EmailConfirmed { get; set; }
        public string UserName { get; set; }
        public string AvatarUrl { get; set; }
        public string AvatarPublicId { get; set; }
        public DateTimeOffset LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public ICollection<FingerPrint>? FingerPrints { get; set; }
    }
}
