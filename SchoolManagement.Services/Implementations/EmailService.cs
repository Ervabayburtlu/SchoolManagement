// SchoolManagement.Services/Implementations/EmailService.cs
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SchoolManagement.Core.Interfaces;
using SchoolManagement.Core.Interfaces.Repositories;

namespace SchoolManagement.Services.Implementations;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendExamReminderAsync(string toEmail, string studentName, string examName, DateTime examDate)
    {
        var host     = _config["EmailSettings:Host"]!;
        var port     = int.Parse(_config["EmailSettings:Port"]!);
        var username = _config["EmailSettings:Username"]!;
        var password = _config["EmailSettings:Password"]!;
        var sender   = _config["EmailSettings:SenderEmail"]!;
        var appUrl   = _config["EmailSettings:AppUrl"]!;

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("BÖYÜ Sınav Sistemi", sender));
        message.To.Add(new MailboxAddress(studentName, toEmail));
        message.Subject = $"⚠️ Katılım Bildirimi Hatırlatması: {examName}";

        message.Body = new BodyBuilder
        {
            HtmlBody = BuildHtml(studentName, examName, examDate, appUrl),
            TextBody = $"Sayın {studentName}, {examName} sınavına {examDate:dd.MM.yyyy HH:mm} tarihinde " +
                       $"katılım bildiriminizi yapmadınız. Lütfen sisteme girin."
        }.ToMessageBody();

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(host, port, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(username, password);
        await smtp.SendAsync(message);
        await smtp.DisconnectAsync(true);

        _logger.LogInformation("Hatırlatma emaili gönderildi → {Email} | Sınav: {Exam}", toEmail, examName);
    }

    private static string BuildHtml(string name, string exam, DateTime date, string appUrl) => $"""
        <!DOCTYPE html><html lang="tr"><head><meta charset="UTF-8"></head>
        <body style="font-family:Arial,sans-serif;background:#f5f5f5;padding:20px;">
          <div style="max-width:600px;margin:auto;background:#fff;border-radius:8px;box-shadow:0 2px 8px rgba(0,0,0,.1);">
            <div style="background:#1a3a6b;padding:24px;text-align:center;">
              <h1 style="color:#fff;margin:0;font-size:20px;">Bandırma Onyedi Eylül Üniversitesi</h1>
              <p style="color:#a8c4e0;margin:4px 0 0;">Sınav Katılım Takip Sistemi</p>
            </div>
            <div style="padding:32px;">
              <h2 style="color:#c0392b;margin-top:0;">⚠️ Sınav Katılım Bildirimi Yapılmadı</h2>
              <p>Sayın <strong>{name}</strong>,</p>
              <p>Aşağıdaki sınava <strong>2 günden az</strong> süre kaldı ve henüz bildirim yapmadınız:</p>
              <div style="background:#fff3cd;border-left:4px solid #ffc107;padding:16px;border-radius:4px;margin:20px 0;">
                <p style="margin:0;"><strong>📚 Sınav:</strong> {exam}</p>
                <p style="margin:8px 0 0;"><strong>📅 Tarih:</strong> {date:dd MMMM yyyy, HH:mm}</p>
              </div>
              <p style="color:#c0392b;"><strong>⚠️ Bildirim yapmazsanız Tutarlılık Puanınızdan 1 hak düşecektir.</strong></p>
              <div style="text-align:center;margin:28px 0;">
                <a href="{appUrl}/sinav-katilim"
                   style="background:#1a3a6b;color:#fff;padding:14px 32px;text-decoration:none;border-radius:6px;font-size:16px;display:inline-block;">
                  Sisteme Git →
                </a>
              </div>
            </div>
            <div style="background:#f0f0f0;padding:16px;text-align:center;font-size:12px;color:#888;">
              Bu email otomatik gönderilmiştir. Lütfen yanıtlamayınız.
            </div>
          </div>
        </body></html>
        """;
}