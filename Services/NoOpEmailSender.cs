using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Threading.Tasks;

public class NoOpEmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        // Apenas imprime no console (opcional). Não envia e-mail real.
        Console.WriteLine($"[FAKE EMAIL] Para: {email}, Assunto: {subject}");
        return Task.CompletedTask;
    }
}