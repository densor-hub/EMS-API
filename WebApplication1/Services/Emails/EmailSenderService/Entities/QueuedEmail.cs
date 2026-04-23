using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMS.Modules.Licenses.Core.Emails.EmailSenderService.Entities
{
    public class QueuedEmail
    {
        public Guid Id { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string TemplateName { get; set; } // "VisitInvitation", "WelcomeEmail", etc.
        public string TemplateModelJson { get; set; } // Serialized template model
        public string TemplateModelType { get; set; } // Assembly qualified type name
        public DateTime CreatedAt { get; set; }
        public DateTime? ScheduledFor { get; set; }
        public DateTime? SentAt { get; set; }
        public DateTime? LastAttemptAt { get; set; }
        public int RetryCount { get; set; }
        public string ErrorMessage { get; set; }
        public EmailQueueStatus Status { get; set; }

        // Foreign keys for tracking
        public Guid? CustomerId { get; set; }
        public Guid? EmployeeId { get; set; }

        // Constants
        public const int MaxRetryAttempts = 3;

        public bool CanRetry => RetryCount < MaxRetryAttempts &&
                                Status != EmailQueueStatus.Sent;
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
