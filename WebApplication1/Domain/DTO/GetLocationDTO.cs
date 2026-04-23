using System.ComponentModel.DataAnnotations;
using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.DTO
{
    public class GetLocationDTO
    {
        public Guid Id { get; set; }
        public string Code { get;  set; }
        public string Name { get;  set; }
        public string? Address { get;  set; }
        public string Phone { get;  set; }
        public string? Email { get;  set; }
        public bool Status { get;  set; }
        public string TypeOfLocationName { get; set; }
        public LocationType TypeOfLocation { get;  set; }
        public  List<IdAndNameDTO> Managers { get; set; }
    }
}
