using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VMS.Modules.Licenses.Core.Emails.EmailSenderService.Entities;

namespace WebApplication1.Services.Emails.EmailService.Queuer
{
    public interface IEmailQueueRepository
    {
        // ── Existing (kept for other callers) ──
        Task AddAsync(QueuedEmail email);
        Task AddRangeAsync(IEnumerable<QueuedEmail> emails);
        //Task<List<QueuedEmail>> GetPendingAsync(int batchSize);
        Task UpdateAsync(QueuedEmail email);
        Task<int> RecoverStuckEmailsAsync(TimeSpan stuckThreshold, CancellationToken ct = default);

        // ── NEW — atomic, short-lived, used by EmailProcessor ──
        Task<List<Guid>> GetPendingIdsAsync(int batchSize);
        Task<QueuedEmail?> TryClaimPendingAsync(Guid id, DateTime claimedAt, CancellationToken ct = default);
        Task MarkSentAsync(Guid id, DateTime sentAt, CancellationToken ct = default);
        Task MarkFailedAsync(Guid id, int retryCount, string errorMessage, CancellationToken ct = default);
        Task MarkRetryAsync(Guid id, int retryCount, DateTime scheduledFor, string errorMessage, CancellationToken ct = default);
    }
}