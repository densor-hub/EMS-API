using System;


namespace WebApplication1.Services.Hubs.DTO;

internal class NotificationDto
{
    public Guid Id { get; set; }
    public string SenderName { get; set; }
    public string CreatedBy { get; set; }
    public string Message { get; set; }
    public string Type { get; set; }
    public string Reference { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Path { get; set; }
    public Guid? ActivityReferenceId { get; set; }
}