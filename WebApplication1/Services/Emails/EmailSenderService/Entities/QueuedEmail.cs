using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace VMS.Modules.Licenses.Core.Emails.EmailSenderService.Entities
{
    public class QueuedEmail
    {
        public Guid Id { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }

        public string TemplateName { get; set; }       // "VisitInvitation", "WelcomeEmail", etc.
        public string TemplateModelJson { get; set; }  // Serialized template model (or envelope)
        public string TemplateModelType { get; set; }  // Assembly qualified type name

        public DateTime CreatedAt { get; set; }
        public DateTime? ScheduledFor { get; set; }
        public DateTime? SentAt { get; set; }
        public DateTime? LastAttemptAt { get; set; }
        public int RetryCount { get; set; }
        public string? ErrorMessage { get; set; }
        public EmailQueueStatus Status { get; set; }

        // Foreign keys for tracking
        public Guid? ReceiverId { get; set; }

        // Constants
        public const int MaxRetryAttempts = 3;

        public bool CanRetry =>
            RetryCount < MaxRetryAttempts &&
            Status != EmailQueueStatus.Sent &&
            Status != EmailQueueStatus.Cancelled;

        /// <summary>
        /// BCC recipients, read out of the envelope stored in <see cref="TemplateModelJson"/>.
        /// Returns an empty list for legacy rows (raw model JSON, no envelope) or malformed JSON.
        /// Never null.
        /// </summary>
        [NotMapped]
        public List<string> Bcc
        {
            get
            {
                if (string.IsNullOrWhiteSpace(TemplateModelJson))
                    return new List<string>();

                try
                {
                    using var doc = JsonDocument.Parse(TemplateModelJson);

                    if (doc.RootElement.ValueKind != JsonValueKind.Object)
                        return new List<string>();

                    if (!doc.RootElement.TryGetProperty("bccList", out var b))
                        return new List<string>();

                    if (b.ValueKind != JsonValueKind.Array)
                        return new List<string>();

                    return b.EnumerateArray()
                        .Where(x => x.ValueKind == JsonValueKind.String)
                        .Select(x => x.GetString())
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .ToList()!;
                }
                catch (JsonException)
                {
                    // Malformed JSON — no BCC available. The processor will surface
                    // the real error when it tries to deserialize the model.
                    return new List<string>();
                }
            }
        }
    }

    public enum EmailQueueStatus
    {
        Pending,
        Processing,
        Sent,
        Failed,
        Cancelled
    }
}