using System;
using MailKit.Net.Smtp;
using MimeKit;


public static class EmailService
{
    public static void SendResetEmail(string toEmail, string token)
    {
        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse("cudianot@gmail.com"));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = "Password Reset";
        message.Body = new TextPart("plain")
        {
            Text = $"Your password reset code is: {token}\nThis code expires in 30 minutes."
        };

        using var client = new SmtpClient();
        client.Connect("smtp.gmail.com", 587, false);
        client.Authenticate(
            Environment.GetEnvironmentVariable("SMTP_USER"),
            Environment.GetEnvironmentVariable("SMTP_PASSWORD")
        );
        client.Send(message);
        client.Disconnect(true);
    }
}