using Apex_Website_API.Model;
using Apex_Website_API.Repositories.Interfaces;
using Apex_Website_API.Services.Interfaces; // <- replace with the actual namespace

namespace Apex_Website_API.Services.Implementations
{
    public class CareerService : ICareerService
    {
        private readonly ICareerRepository _careerRepository;
        private readonly IS3Service _s3Service;
        private readonly IEmailService _emailService;
        private readonly ILogger<CareerService> _logger;

        public CareerService(ICareerRepository careerRepository, IEmailService emailService, IS3Service s3Service, ILogger<CareerService> logger)
        {
            _careerRepository = careerRepository;
            _emailService = emailService;
            _s3Service = s3Service;
            _logger = logger;
        }

        public async Task<Guid> SaveCareerAsync(Career career,string requestJson)
        {
            var responseJson = """
                
                {
                    "success": true,
                    "message": "Career information saved successfully."
                }
                """;
            try
            {
                _logger.LogInformation("CAREER SERVICE | Save started");

                // ==========================================
                // STEP 1: Validate Resume
                // ==========================================

                if (career.UploadResume == null || career.UploadResume.Length == 0)
                {
                    throw new ArgumentException("Resume file is required.");
                }

                if (career.UploadResume.Length > 2 * 1024 * 1024)
                {
                    throw new ArgumentException("Resume file size cannot exceed 2 MB.");
                }

                _logger.LogInformation("CAREER SERVICE | Resume validation successful | FileName: {FileName}",career.UploadResume.FileName);

                // ==========================================
                // STEP 2: Upload Resume to AWS S3
                // ==========================================

                var s3Key = await _s3Service.UploadResumeAsync(career.UploadResume);

                _logger.LogInformation("CAREER SERVICE | Resume uploaded to S3 | Key: {S3Key}",s3Key);

                // ==========================================
                // STEP 3: Save Career Information to Database
                // ==========================================

                var careerId =await _careerRepository.SaveCareerAsync(career,requestJson,responseJson,s3Key);

                _logger.LogInformation("CAREER SERVICE | Database save successful | CareerId: {CareerId}",careerId);

                _logger.LogInformation("CAREER SERVICE | Save completed | CareerId: {CareerId}",careerId);

                return careerId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"CAREER SERVICE | Save failed");
                throw;
            }
        }


        //public async Task<Guid> SaveCareerAsync(Career career,string requestJson)
        //{
        //    var responseJson = """
        //    {
        //        "success": true,
        //        "message": "Career information saved successfully."
        //    }
        //    """;

        //    _logger.LogInformation("CAREER SERVICE | Save started");

        //    // ==========================================
        //    // STEP 1: Create Base64 from uploaded file
        //    // ==========================================

        //    string resumeBase64;

        //    if (career.UploadResume == null || career.UploadResume.Length == 0)
        //    {
        //        throw new ArgumentException("Resume file is required.");
        //    }

        //    if (career.UploadResume.Length > 2 * 1024 * 1024)
        //    {
        //        throw new ArgumentException("Resume file size cannot exceed 2 MB.");
        //    }

        //    await using (var memoryStream = new MemoryStream())
        //    {
        //        await career.UploadResume.CopyToAsync(memoryStream);

        //        resumeBase64 = Convert.ToBase64String(memoryStream.ToArray());
        //    }

        //    _logger.LogInformation("CAREER SERVICE | Resume converted to Base64 | FileName: {FileName}",career.UploadResume.FileName);

        //    // Upload Resume to AWS S3
        //    //var s3Key = await _s3Service.UploadResumeAsync(career.UploadResume);

        //    //_logger.LogInformation("CAREER SERVICE | Resume uploaded to S3 | Key: {S3Key}",s3Key);

        //    // Save Career Information to Database
        //    //var careerId = Guid.NewGuid();
        //    //var careerId = await _careerRepository.SaveCareerAsync(career,requestJson,responseJson);
        //    var careerId = await _careerRepository.SaveCareerAsync(career,requestJson,responseJson,resumeBase64);

        //    // ==========================================
        //    // STEP 3: Send email ONLY after DB success
        //    // ==========================================
        //    //await _emailService.TestGmailSmtpAsync();
        //    //await _emailService.SendCareerEmailAsync("amit.yadav@apexkidneycare.in", $"New Career Application - {career.ApplyForPosition}"
        //    //    , $"""
        //    //        <html>
        //    //        <body>
        //    //            <h3>New Career Application</h3>

        //    //            <p><b>Name:</b> {career.FullName}</p>
        //    //            <p><b>Email:</b> {career.EmailId}</p>
        //    //            <p><b>Phone:</b> {career.PhoneNumber}</p>
        //    //            <p><b>Position:</b> {career.ApplyForPosition}</p>
        //    //            <p><b>Experience:</b> {career.Experience}</p>
        //    //            <p><b>Notice Period:</b> {career.NoticePeriod}</p>
        //    //            <p><b>Message:</b> {career.Message}</p>
        //    //        </body>
        //    //        </html>
        //    //        """,
        //    //    resumeBase64,
        //    //    career.UploadResume.FileName,
        //    //    career.UploadResume.ContentType);

        //    _logger.LogInformation("CAREER SERVICE | Save completed | CareerId: {CareerId}",careerId);

        //    return careerId;
        //}


        //public async Task<Guid> SaveCareerAsync(Career career, string requestJson)
        //{
        //    var responseJson = """
        //    {
        //        "success": true,
        //        "message": "Career information saved successfully."
        //    }
        //    """;
        //    _logger.LogInformation("CAREER SERVICE | Save started");
        //    var careerId = await _careerRepository.SaveCareerAsync(career, requestJson, responseJson);
        //    _logger.LogInformation("CAREER SERVICE | Save completed");
        //    return careerId;
        //}
    }

}
