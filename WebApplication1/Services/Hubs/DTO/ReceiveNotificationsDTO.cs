using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1.Services.Hubs.DTO
{
    internal class ReceiveNotificationsDTO
    {
        public Guid CompanyId { get; set; }
        public Guid AppId { get; set; }
        public string Room { get; set; }
        public Guid NotificationId { get; set; }
      
    }
}
