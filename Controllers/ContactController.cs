using Apex_Website_API.Model;
using Apex_Website_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace Apex_Website_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ContactController : ControllerBase
    {
        private readonly IContactService _contactService;
        private readonly ILogger<ContactController> _logger;

        public ContactController(IContactService contactService,ILogger<ContactController> logger)
        {
            _contactService = contactService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> SaveContact([FromBody] JsonElement requestBody)
        {
            _logger.LogInformation("CONTACT | Request received");

            string requestJson = requestBody.GetRawText();

            var contact = JsonSerializer.Deserialize<Contact>(requestJson,new JsonSerializerOptions
                                                                         {
                                                                             PropertyNameCaseInsensitive = true
                                                                         });
            
            if (contact == null)
            {
                _logger.LogWarning("CONTACT | Invalid request");

                return BadRequest(new
                {
                    success = false,
                    message = "Invalid contact request."
                });
            }

            var contactId = await _contactService.SaveContactAsync(contact,requestJson);

            _logger.LogInformation("CONTACT | Save successful | ContactId: {ContactId}",contactId);

            return Ok(new
            {
                success = true,
                message = "Contact information saved successfully.",
                contactId = contactId
            });
        }
    }
}