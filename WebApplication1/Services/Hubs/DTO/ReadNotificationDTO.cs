using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1.Services.Hubs.DTO
{
    internal class ReadNotificationDTO
    {
        public Guid ConnectionId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid AppId { get; set; }
        public Guid? NotificationId { get; set; } = Guid.Empty;
        public string Room { get; set; }
    }
}
