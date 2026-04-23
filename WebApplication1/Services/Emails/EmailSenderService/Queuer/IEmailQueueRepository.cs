using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.Modules.Licenses.Core.Emails.EmailSenderService.Entities;

namespace WebApplication1.Services.Emails.EmailService.Queuer
{
    public interface IEmailQueueRepository
    {
        Task AddAsync(QueuedEmail email);
        Task AddRangeAsync(IEnumerable<QueuedEmail> emails);
        Task<List<QueuedEmail>> GetPendingAsync(int batchSize);
        Task UpdateAsync(QueuedEmail email);

        Task DeleteSentEmails();
    }

}
