using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolManagement.Core.Common
{
    public class AccountLockedException : Exception
    {
        public string? AdvisorName { get; }

        public AccountLockedException(string? advisorName = null)
            : base("Hesabınız kilitlenmiştir.")
        {
            AdvisorName = advisorName;
        }
    }
}
