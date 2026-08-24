namespace Apex_Website_API.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendCareerEmailAsync(string toEmail,string subject,string body,string resumeBase64,string fileName,string contentType);

        Task TestGmailSmtpAsync();
    }
}
