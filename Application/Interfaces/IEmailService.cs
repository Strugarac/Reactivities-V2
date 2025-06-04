using System;

namespace Application.Interfaces;

public interface IEmailService
{
    Task SendConfirmationLinkAsync(string to, string subject, string confirmationLink);
}
