using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.Modules.Licenses.Core.Emails.EmailSenderService.Entities;
using WebApplication1.DAL;

namespace WebApplication1.Services.Emails.EmailService.Queuer
{
    internal class EmailQueueRepository : IEmailQueueRepository
    {
        private readonly AppDbContext _context;

        public EmailQueueRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(QueuedEmail email)
        {
            email.Id = Guid.NewGuid();
            email.CreatedAt = DateTime.UtcNow;
            email.Status = EmailQueueStatus.Pending;
            email.RetryCount = 0;

            await _context.Set<QueuedEmail>().AddAsync(email);
            await _context.SaveChangesAsync();
        }

        public async Task<List<QueuedEmail>> GetPendingAsync(int batchSize)
        {
            var now = DateTime.UtcNow;

            return await _context.Set<QueuedEmail>()
                .Where(e => e.Status == EmailQueueStatus.Pending &&
                           (e.ScheduledFor == null || e.ScheduledFor <= now))
                .OrderBy(e => e.CreatedAt)
                .Take(batchSize)
                .ToListAsync();
        }

        public async Task UpdateAsync(QueuedEmail email)
        {
            _context.Set<QueuedEmail>().Update(email);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<QueuedEmail> emails)
        {
            await _context.QueuedEmails.AddRangeAsync(emails);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSentEmails()
        {
            var sentEmails = _context.QueuedEmails.Where(x => x.Status == EmailQueueStatus.Sent);

            _context.RemoveRange(sentEmails);

            await _context.SaveChangesAsync();
        }
    }
}
