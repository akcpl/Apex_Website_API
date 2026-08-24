using System.ComponentModel.DataAnnotations;

namespace Apex_Website_API.Model
{
    public class Contact
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string City { get; set; } = string.Empty;

        public string? Query { get; set; }
    }
}
