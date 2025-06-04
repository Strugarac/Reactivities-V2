using System;

namespace Application.Configuration;

public class EmailSettings
{
    public required string SmtpServer { get; set; }
    public int Port { get; set; }
    public bool UseSsl { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string SenderEmail { get; set; }
    public required string SenderName { get; set; } 
}
