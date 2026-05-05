using SchoolManagement.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolManagement.Services.Interfaces
{
    public interface IConsistencyService
    {
        Task OnExcuseSubmittedAsync(string studentNo);
        Task OnExcuseApprovedAsync(string studentNo);
        Task OnExcuseRejectedAsync(string studentNo);
        Task UnlockAccountAsync(string studentNo);
        Task SetBarCountAsync(string studentNo, int count);
        Task<Student?> GetRecordAsync(string studentNo);
        Task OnAbsentWithoutNotificationAsync(string studentNo);
        Task OnInconsistentBehaviorAsync(string studentNo);
    }
}
