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

            await _context.QueuedEmails.AddAsync(email);
        }

        public async Task<List<QueuedEmail>> GetPendingAsync(int batchSize)
        {
            var now = DateTime.UtcNow;

            return await _context.QueuedEmails
                .Where(e => e.Status == EmailQueueStatus.Pending &&
                           (e.ScheduledFor == null || e.ScheduledFor <= now))
                .OrderBy(e => e.CreatedAt)
                .Take(batchSize)
                .ToListAsync();
        }

        public async Task UpdateAsync(QueuedEmail email)
        {
            _context.QueuedEmails.Update(email);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<QueuedEmail> emails)
        {
            await _context.QueuedEmails.AddRangeAsync(emails);
            // await _context.SaveChangesAsync();
        }

        // 1. Purge sent emails older than the retention window (audit-safe)
        public async Task<int> DeleteSentEmailsAsync(TimeSpan retention)
        {
            var cutoff = DateTime.UtcNow - retention;

            return await _context.QueuedEmails
                .Where(x => x.Status == EmailQueueStatus.Sent
                            && x.SentAt != null
                            && x.SentAt < cutoff)
                .ExecuteDeleteAsync();
        }

        // 2. Purge dead emails (failed forever, or stuck Processing too long)
        public async Task<int> DeleteStaleEmailsAsync(TimeSpan age)
        {
            var cutoff = DateTime.UtcNow - age;

            return await _context.QueuedEmails
                .Where(x => (x.Status == EmailQueueStatus.Failed
                             || x.Status == EmailQueueStatus.Processing)
                            && x.CreatedAt < cutoff)
                .ExecuteDeleteAsync();
        }

        // 3. Recover stuck Processing emails (the real bug fix)
        public async Task<int> RecoverStuckEmailsAsync(
            TimeSpan stuckThreshold,
            CancellationToken ct = default)
        {
            var cutoff = DateTime.UtcNow - stuckThreshold;

            return await _context.QueuedEmails
                .Where(e => e.Status == EmailQueueStatus.Processing
                            && e.LastAttemptAt != null
                            && e.LastAttemptAt < cutoff)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(e => e.Status, EmailQueueStatus.Pending)
                    .SetProperty(e => e.ScheduledFor, DateTime.UtcNow),
                    ct);
        }

    }
}
