using System.ComponentModel.DataAnnotations;

namespace Apex_Website_API.Model
{
    public class Career
    {
        [Required]
        public string ApplyForPosition { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        public string EmailId { get; set; }

        public string Experience { get; set; }

        [Required]
        public string NoticePeriod { get; set; }

        public string Message { get; set; }

        [Required]
        public IFormFile UploadResume { get; set; }
    }
}
