using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        // ═══════════════════════════════════════════════════════════════
        // Existing methods (unchanged)
        // ═══════════════════════════════════════════════════════════════

        public async Task AddAsync(QueuedEmail email)
        {
            email.Id = Guid.NewGuid();
            email.CreatedAt = DateTime.UtcNow;
            email.Status = EmailQueueStatus.Pending;
            email.RetryCount = 0;

            await _context.QueuedEmails.AddAsync(email);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<QueuedEmail> emails)
        {
            var list = emails.ToList();
            var now = DateTime.UtcNow;
            foreach (var e in list)
            {
                if (e.Id == Guid.Empty) e.Id = Guid.NewGuid();
                if (e.CreatedAt == default) e.CreatedAt = now;
                e.Status = EmailQueueStatus.Pending;
                if (e.RetryCount < 0) e.RetryCount = 0;
            }
            await _context.QueuedEmails.AddRangeAsync(list);
            await _context.SaveChangesAsync();
        }

        // ⚠️ LEGACY — do not use from EmailProcessor.
        // public async Task<List<QueuedEmail>> GetPendingAsync(int batchSize)
        // {
        //     var now = DateTime.UtcNow;
        //     return await _context.QueuedEmails
        //         .AsNoTracking()
        //         .Where(e => e.Status == EmailQueueStatus.Pending &&
        //                     (e.ScheduledFor == null || e.ScheduledFor <= now))
        //         .OrderBy(e => e.CreatedAt)
        //         .Take(batchSize)
        //         .ToListAsync();
        // }

        // ⚠️ LEGACY — do not use from EmailProcessor.
        // Tracking-based update; callers MUST keep the DbContext alive across the call.
        // New code should use MarkSentAsync / MarkFailedAsync / MarkRetryAsync instead.
        public async Task UpdateAsync(QueuedEmail email)
        {
            // If the entity isn't already tracked, this attaches it as Modified.
            if (_context.Entry(email).State == EntityState.Detached)
                _context.QueuedEmails.Update(email);

            await _context.SaveChangesAsync();
        }

        // ═══════════════════════════════════════════════════════════════
        // NEW — atomic, short-lived
        // ═══════════════════════════════════════════════════════════════

        public async Task<List<Guid>> GetPendingIdsAsync(int batchSize)
        {
            var now = DateTime.UtcNow;
            return await _context.QueuedEmails
                .AsNoTracking()
                .Where(e => e.Status == EmailQueueStatus.Pending &&
                            (e.ScheduledFor == null || e.ScheduledFor <= now))
                .OrderBy(e => e.CreatedAt)
                .Take(batchSize)
                .Select(e => e.Id)
                .ToListAsync();
        }

        public async Task<QueuedEmail?> TryClaimPendingAsync(
            Guid id, DateTime claimedAt, CancellationToken ct = default)
        {
            // Atomic UPDATE: only succeeds if the row is still Pending.
            // ExecuteUpdateAsync returns the number of rows affected.
            var affected = await _context.QueuedEmails
                .Where(e => e.Id == id && e.Status == EmailQueueStatus.Pending)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(e => e.Status, EmailQueueStatus.Processing)
                    .SetProperty(e => e.LastAttemptAt, claimedAt),
                    ct);

            if (affected == 0)
                return null;

            // Read back as detached — safe to hand off across scopes.
            return await _context.QueuedEmails
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id, ct);
        }

        public async Task MarkSentAsync(Guid id, DateTime sentAt, CancellationToken ct = default)
        {
            await _context.QueuedEmails
                .Where(e => e.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(e => e.Status, EmailQueueStatus.Sent)
                    .SetProperty(e => e.SentAt, sentAt)
                    .SetProperty(e => e.ErrorMessage, (string?)null),
                    ct);
        }

        public async Task MarkFailedAsync(
            Guid id, int retryCount, string errorMessage, CancellationToken ct = default)
        {
            await _context.QueuedEmails
                .Where(e => e.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(e => e.Status, EmailQueueStatus.Failed)
                    .SetProperty(e => e.RetryCount, retryCount)
                    .SetProperty(e => e.ErrorMessage, errorMessage),
                    ct);
        }

        public async Task MarkRetryAsync(
            Guid id, int retryCount, DateTime scheduledFor, string errorMessage,
            CancellationToken ct = default)
        {
            await _context.QueuedEmails
                .Where(e => e.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(e => e.Status, EmailQueueStatus.Pending)
                    .SetProperty(e => e.RetryCount, retryCount)
                    .SetProperty(e => e.ScheduledFor, scheduledFor)
                    .SetProperty(e => e.ErrorMessage, errorMessage),
                    ct);
        }

        // ═══════════════════════════════════════════════════════════════
        // Existing purge / recovery
        // ═══════════════════════════════════════════════════════════════

        // public async Task<int> DeleteSentEmailsAsync(TimeSpan retention)
        // {
        //     var cutoff = DateTime.UtcNow - retention;
        //     return await _context.QueuedEmails
        //         .Where(x => x.Status == EmailQueueStatus.Sent
        //                     && x.SentAt != null
        //                     && x.SentAt < cutoff)
        //         .ExecuteDeleteAsync();
        // }

        public async Task<int> DeleteStaleEmailsAsync(TimeSpan age)
        {
            var cutoff = DateTime.UtcNow - age;
            return await _context.QueuedEmails
                .Where(x => (x.Status == EmailQueueStatus.Failed
                             || x.Status == EmailQueueStatus.Processing)
                            && x.CreatedAt < cutoff)
                .ExecuteDeleteAsync();
        }

        public async Task<int> RecoverStuckEmailsAsync(
            TimeSpan stuckThreshold, CancellationToken ct = default)
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