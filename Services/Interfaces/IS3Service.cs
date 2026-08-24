
using Microsoft.AspNetCore.Http;
namespace Apex_Website_API.Services.Interfaces
{
    public interface IS3Service
    {
        Task<string> UploadResumeAsync(IFormFile file);
    }
}
