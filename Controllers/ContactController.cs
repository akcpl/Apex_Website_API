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

        public ContactController(IContactService contactService, ILogger<ContactController> logger)
        {
            _contactService = contactService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> SaveContact(
            [FromBody] JsonElement requestBody)
        {
            try
            {
                _logger.LogInformation("CONTACT | Request received");

                // ==========================================
                // STEP 1: Get Request JSON
                // ==========================================

                string requestJson = requestBody.GetRawText();

                // ==========================================
                // STEP 2: Deserialize Request
                // ==========================================

                var contact = JsonSerializer.Deserialize<Contact>(requestJson,
                            new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            });

                // ==========================================
                // STEP 3: Validate Request
                // ==========================================

                if (contact == null)
                {
                    _logger.LogWarning("CONTACT | Invalid request");

                    return BadRequest(new
                    {
                        success = false,
                        message = "Invalid contact request."
                    });
                }

                // ==========================================
                // STEP 4: Save Contact
                // ==========================================

                var contactId = await _contactService.SaveContactAsync(contact, requestJson);

                _logger.LogInformation("CONTACT | Save successful | ContactId: {ContactId}", contactId);

                // ==========================================
                // STEP 5: Success Response
                // ==========================================

                return Ok(new
                {
                    success = true,
                    message = "Contact information saved successfully.",
                    contactId = contactId
                });
            }
            catch (ArgumentException ex)
            {
                // ==========================================
                // 400 - Validation Error
                // ==========================================

                _logger.LogWarning(ex, "CONTACT | Validation error");

                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (JsonException ex)
            {
                // ==========================================
                // 400 - Invalid JSON
                // ==========================================

                _logger.LogWarning(ex, "CONTACT | Invalid JSON request");

                return BadRequest(new
                {
                    success = false,
                    message = "Invalid JSON request."
                });
            }
            catch (Exception ex)
            {
                // ==========================================
                // 500 - Unexpected Error
                // ==========================================

                _logger.LogError(ex, "CONTACT | Unexpected error");

                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        success = false,
                        message = ex.Message
                    });
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> SaveContact([FromBody] JsonElement requestBody)
        //{
        //    _logger.LogInformation("CONTACT | Request received");

        //    string requestJson = requestBody.GetRawText();

        //    var contact = JsonSerializer.Deserialize<Contact>(requestJson,new JsonSerializerOptions
        //                                                                 {
        //                                                                     PropertyNameCaseInsensitive = true
        //                                                                 });

        //    if (contact == null)
        //    {
        //        _logger.LogWarning("CONTACT | Invalid request");

        //        return BadRequest(new
        //        {
        //            success = false,
        //            message = "Invalid contact request."
        //        });
        //    }

        //    var contactId = await _contactService.SaveContactAsync(contact,requestJson);

        //    _logger.LogInformation("CONTACT | Save successful | ContactId: {ContactId}",contactId);

        //    return Ok(new
        //    {
        //        success = true,
        //        message = "Contact information saved successfully.",
        //        contactId = contactId
        //    });
        //}
    }
}