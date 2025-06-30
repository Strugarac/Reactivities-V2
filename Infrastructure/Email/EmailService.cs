using System;
using Application.Configuration;
using Application.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Infrastructure.Email;

public class EmailService(IOptions<EmailSettings> options ) : IEmailService
{
    public async Task SendEmailAsync(string to, string subject, string confirmationLink)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress(options.Value.SenderName, options.Value.SenderEmail));

        message.To.Add(MailboxAddress.Parse(to));

        message.Subject = subject;
        message.Body = new TextPart("html")
        {
            Text = confirmationLink
        };

        using var client = new SmtpClient();

        await client.ConnectAsync(options.Value.SmtpServer, options.Value.Port, MailKit.Security.SecureSocketOptions.StartTls);

        await client.AuthenticateAsync(options.Value.Username, options.Value.Password);

        Console.WriteLine(message);
        //await client.SendAsync(message);

        await client.DisconnectAsync(true);
    }
}
