using Apex_Website_API.Services.Interfaces;
using System.Net;
using System.Net.Mail;
using MailKit.Security;
using MimeKit;

namespace Apex_Website_API.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendCareerEmailAsync(string toEmail, string subject, string body, string resumeBase64, string fileName, string contentType)
        {
            try
            {
                _logger.LogInformation("CAREER EMAIL | Email sending started");

                // Convert Base64 Resume to Bytes
                byte[] fileBytes = Convert.FromBase64String(resumeBase64);

                using var stream = new MemoryStream(fileBytes);

                using var attachment = new Attachment(stream, fileName, contentType);

                // Create Mail
                using var mail = new MailMessage();

                // Sender
                mail.From = new MailAddress(_configuration["SMTP:FromEmail"]!, "Website-Quick Contact- AKC");

                // Recipient
                mail.To.Add(toEmail);

                // CC
                mail.CC.Add(_configuration["SMTP:CCEmail"]!);

                // Reply To
                mail.ReplyToList.Add(new MailAddress(toEmail));

                // Subject
                mail.Subject = subject;

                // Body
                mail.Body = body;
                mail.IsBodyHtml = true;

                // Attachment
                mail.Attachments.Add(attachment);

                // Gmail SMTP
                using var smtp = new SmtpClient(_configuration["SMTP:Host"]!);

                // SMTP Port
                smtp.Port = int.Parse(_configuration["SMTP:Port"]!);
                //smtp.Port = 465;

                // SMTP Authentication
                smtp.Credentials = new NetworkCredential(_configuration["SMTP:UserId"]!, _configuration["SMTP:Password"]!);
                //smtp.Credentials = new NetworkCredential("info@apexkidneycare.com",_configuration["SMTP:Password"]!.Replace(" ", ""));

                // TLS
                //smtp.EnableSsl = bool.Parse(_configuration["SMTP:EnableSsl"]!);
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;                

                //var smtpHost = _configuration["SMTP:Host"];
                //var smtpPort = _configuration["SMTP:Port"];
                //var smtpUser = _configuration["SMTP:UserId"];
                //var smtpPassword = _configuration["SMTP:Password"];
                //var smtpSsl = _configuration["SMTP:EnableSsl"];
                //_logger.LogInformation("SMTP CONFIG | Host: {Host} | Port: {Port} | User: {User} | PasswordConfigured: {PasswordConfigured} | SSL: {SSL}",smtpHost,smtpPort,smtpUser,!string.IsNullOrWhiteSpace(smtpPassword),smtpSsl);

                // Send Email
                await smtp.SendMailAsync(mail);

                _logger.LogInformation("CAREER EMAIL | Email sent successfully | Attachment: {FileName}", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CAREER EMAIL | Email sending failed");
                throw;
            }
        }

        public async Task TestGmailSmtpAsync()
        {
            try
            {
                _logger.LogInformation(
                    "MAILKIT TEST | Gmail SMTP test started");

                var message = new MimeMessage();

                // From
                message.From.Add(
                    new MailboxAddress(
                        "Website-Quick Contact- AKC",
                        _configuration["SMTP:FromEmail"]!
                    )
                );

                // To
                message.To.Add(
                    MailboxAddress.Parse(
                        "info@apexkidneycare.com"
                    )
                );

                // Subject
                message.Subject = "Gmail SMTP Test";

                // Body
                message.Body = new TextPart("html")
                {
                    Text = "<h3>Test email from Apex Website API</h3>"
                };

                // MailKit SMTP Client
                using var smtp = new MailKit.Net.Smtp.SmtpClient();

                // Connect using STARTTLS
                _logger.LogInformation(
                    "MAILKIT TEST | Connecting to smtp.gmail.com:587");

                await smtp.ConnectAsync(
                    "smtp.gmail.com",
                    587,
                    SecureSocketOptions.StartTls
                );

                _logger.LogInformation(
                    "MAILKIT TEST | Connected | Secure: {Secure}",
                    smtp.IsSecure
                );

                // Authenticate
                _logger.LogInformation(
                    "MAILKIT TEST | Authenticating");

                await smtp.AuthenticateAsync(
                    "info@apexkidneycare.com",
                    _configuration["SMTP:Password"]!.Replace(" ", "")
                );

                _logger.LogInformation(
                    "MAILKIT TEST | Authentication successful");

                // Send
                await smtp.SendAsync(message);

                _logger.LogInformation(
                    "MAILKIT TEST | Email sent successfully");

                // Disconnect
                await smtp.DisconnectAsync(true);

                _logger.LogInformation(
                    "MAILKIT TEST | Disconnected");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "MAILKIT TEST | Gmail SMTP test failed"
                );

                throw;
            }
        }
    }



    //public async Task SendCareerEmailAsync(string toEmail,string subject,string body,string resumeBase64,string fileName,string contentType)
    //{
    //    try
    //    {
    //        _logger.LogInformation("CAREER EMAIL | Email sending started");

    //        // Base64 → byte[]
    //        byte[] fileBytes =Convert.FromBase64String(resumeBase64);

    //        using var stream =new MemoryStream(fileBytes);

    //        using var attachment = new Attachment(stream,fileName,contentType);

    //        using var mail = new MailMessage();

    //        mail.From = new MailAddress(_configuration["SMTP:FromEmail"]!);

    //        mail.To.Add(toEmail);
    //        mail.Subject = subject;
    //        mail.Body = body;
    //        mail.IsBodyHtml = true;
    //        mail.Attachments.Add(attachment);

    //        using var smtp = new SmtpClient(_configuration["SMTP:Host"]!);

    //        smtp.Port = int.Parse(_configuration["SMTP:Port"]!);

    //        smtp.Credentials = new NetworkCredential(_configuration["SMTP:UserId"],_configuration["SMTP:Password"]);
    //        smtp.EnableSsl = bool.Parse(_configuration["SMTP:EnableSsl"]!);

    //        await smtp.SendMailAsync(mail);

    //        _logger.LogInformation("CAREER EMAIL | Email sent successfully | Attachment: {FileName}",fileName);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex,"CAREER EMAIL | Email sending failed");
    //        throw;
    //    }
    //}
}

