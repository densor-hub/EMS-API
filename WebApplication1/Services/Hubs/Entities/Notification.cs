using System;

namespace WebApplication1.Services.Hubs.Entities;

public class Notification
{
    public Guid Id { get; private set; }
    public string Message { get; private set; }
    public Guid Sender { get; private set; }
    public string SenderName { get; private set; }
    public Guid Receiver { get; private set; }
    public bool IsRead { get; private set; } = false;   
    public DateTime CreatedAt { get; private set; }
    public string FrontEndPath { get; private set; } 
    public Guid ActivityId { get; private set; } 
    public string Type { get; private set; }


    private Notification()
    {
    }

    public Notification(Guid id, string message, Guid sender, Guid receiver,  DateTime createdAt, string frontEndPath, Guid activityId, string type, string senderName)
    {
        Id = id;
        Message = message;
        Sender = sender;
        CreatedAt = createdAt;
        FrontEndPath = frontEndPath;
        ActivityId = activityId;
        Receiver = receiver;
        Type = type;
        SenderName = senderName;
    }


    internal void Read()
    {
        IsRead = true;
    }
}