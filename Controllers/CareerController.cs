using Apex_Website_API.Model;
using Apex_Website_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Apex_Website_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CareerController : ControllerBase
    {
        private readonly ICareerService _careerService;
        private readonly ILogger<CareerController> _logger;

        public CareerController(ICareerService careerService, ILogger<CareerController> logger)
        {
            _careerService = careerService;
            _logger = logger;
        }

        [HttpPost("SaveCareer")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> SaveCareer([FromForm] Career career)
        {
            try
            {
                _logger.LogInformation("CAREER | Request received");

                // ==========================================
                // STEP 1: Validate Request
                // ==========================================

                if (career == null)
                {
                    _logger.LogWarning("CAREER | Invalid request");

                    return BadRequest(new
                    {
                        success = false,
                        message = "Invalid career request."
                    });
                }

                // ==========================================
                // STEP 2: Validate Resume
                // ==========================================

                if (career.UploadResume == null || career.UploadResume.Length == 0)
                {
                    _logger.LogWarning("CAREER | Resume file missing");

                    return BadRequest(new
                    {
                        success = false,
                        message = "Resume file is required."
                    });
                }

                // ==========================================
                // STEP 3: Create Request JSON
                // ==========================================

                string requestJson = JsonSerializer.Serialize(new
                {
                    career.ApplyForPosition,
                    career.FullName,
                    career.PhoneNumber,
                    career.EmailId,
                    career.Experience,
                    career.NoticePeriod,
                    career.Message,
                    ResumeFileName = career.UploadResume.FileName,
                    ResumeContentType = career.UploadResume.ContentType
                });

                _logger.LogInformation("CAREER | Request parsed | File: {FileName}", career.UploadResume.FileName);

                // ==========================================
                // STEP 4: Save Career
                // ==========================================

                var careerId = await _careerService.SaveCareerAsync(career, requestJson);

                _logger.LogInformation("CAREER | Save successful | CareerId: {CareerId}", careerId);

                // ==========================================
                // STEP 5: Success Response
                // ==========================================

                return Ok(new
                {
                    success = true,
                    message = "Career information saved successfully.",
                    careerId = careerId
                });
            }
            catch (ArgumentException ex)
            {
                // ==========================================
                // 400 - Validation Error
                // ==========================================
                _logger.LogWarning(ex, "CAREER | Validation error");

                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                // ==========================================
                // 500 - Unexpected Error
                // ==========================================

                _logger.LogError(ex, "CAREER | Unexpected error");

                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        success = false,
                        message = ex.Message
                    });
            }
        }
    }

    //Latest This part 
    //[HttpPost("SaveCareer")]
    //[Consumes("multipart/form-data")]
    //public async Task<IActionResult> SaveCareer([FromForm] Career career)
    //{
    //    _logger.LogInformation(
    //        "CAREER | Request received");

    //    if (career == null)
    //    {
    //        _logger.LogWarning(
    //            "CAREER | Invalid request");

    //        return BadRequest(new
    //        {
    //            success = false,
    //            message = "Invalid career request."
    //        });
    //    }

    //    if (career.UploadResume == null || career.UploadResume.Length == 0)
    //    {
    //        _logger.LogWarning("CAREER | Resume file missing");

    //        return BadRequest(new
    //        {
    //            success = false,
    //            message = "Resume file is required."
    //        });
    //    }

    //    string requestJson =
    //        JsonSerializer.Serialize(new
    //        {
    //            career.ApplyForPosition,
    //            career.FullName,
    //            career.PhoneNumber,
    //            career.EmailId,
    //            career.Experience,
    //            career.NoticePeriod,
    //            career.Message,
    //            ResumeFileName = career.UploadResume.FileName,
    //            ResumeContentType = career.UploadResume.ContentType
    //        });

    //    _logger.LogInformation("CAREER | Request parsed | File: {FileName}", career.UploadResume.FileName);

    //    var careerId = await _careerService.SaveCareerAsync(career, requestJson);

    //    _logger.LogInformation("CAREER | Save successful | CareerId: {CareerId}", careerId);

    //    return Ok(new
    //    {
    //        success = true,
    //        message =
    //            "Career information saved successfully.",
    //        careerId = careerId
    //    });
    //}


    //[HttpPost("SaveCareer")]
    //public async Task<IActionResult> SaveCareer([FromBody] JsonElement requestBody)
    //{
    //    _logger.LogInformation("CAREER | Request received");


    //    string requestJson = requestBody.GetRawText();

    //    _logger.LogInformation("CAREER | Request JSON received");

    //    var career = JsonSerializer.Deserialize<Career>(requestJson,new JsonSerializerOptions
    //                                                    {
    //                                                        PropertyNameCaseInsensitive = true
    //                                                    });
    //    if (career == null)
    //    {
    //        _logger.LogWarning("CAREER | Invalid request");

    //        return BadRequest(new
    //        {
    //            success = false,
    //            message = "Invalid career request."
    //        });
    //    }

    //    var careerId = await _careerService.SaveCareerAsync(career,requestJson);

    //    _logger.LogInformation("CAREER | Save successful | CareerId: {CareerId}",careerId);

    //    return Ok(new
    //    {
    //        success = true,
    //        message = "Career information saved successfully.",
    //        careerId = careerId
    //    });
    //}



}