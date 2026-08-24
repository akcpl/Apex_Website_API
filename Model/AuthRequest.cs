using System.ComponentModel.DataAnnotations;

namespace Apex_Website_API.Model
{
    public class AuthRequest
    {
        [Required]
        public string ClientId { get; set; } = string.Empty;

        [Required]
        public string ClientSecret { get; set; } = string.Empty;
    }
}