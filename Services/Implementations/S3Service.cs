using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Apex_Website_API.Services.Interfaces;

namespace Apex_Website_API.Services.Implementations
{
    public class S3Service : IS3Service
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<S3Service> _logger;

        public S3Service(
            IConfiguration configuration,
            ILogger<S3Service> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> UploadResumeAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException(
                    "Resume file is required.");
            }

            if (file.Length > 2 * 1024 * 1024)
            {
                throw new ArgumentException(
                    "Resume file size cannot exceed 2 MB.");
            }

            var accessKey =
                _configuration["S3_Upload:AccessKey"];

            var secretKey =
                _configuration["S3_Upload:SecretKey"];

            var bucketName =
                _configuration["S3_Upload:BucketName"];

            var folder =
                _configuration["S3_Upload:UplaodFolder"];

            if (string.IsNullOrWhiteSpace(accessKey))
                throw new Exception("AS3_UploadWS AccessKey is missing.");

            if (string.IsNullOrWhiteSpace(secretKey))
                throw new Exception("S3_Upload SecretKey is missing.");

            if (string.IsNullOrWhiteSpace(bucketName))
                throw new Exception("S3_Upload BucketName is missing.");

            // AWS Mumbai
            var credentials =
                new BasicAWSCredentials(
                    accessKey,
                    secretKey);

            var s3Client =
                new AmazonS3Client(
                    credentials,
                    RegionEndpoint.APSouth1);

            // Test bucket access first
            var headRequest = new HeadBucketRequest
            {
                BucketName = bucketName
            };

            await s3Client.HeadBucketAsync(headRequest);

            _logger.LogInformation(
                "S3 | Bucket access successful | Bucket: {Bucket}",
                bucketName);

            // Create S3 file path
            var extension =
                Path.GetExtension(file.FileName);

            var key =
                $"{folder}/{Guid.NewGuid()}{extension}";

            _logger.LogInformation(
                "S3 | Upload started | Key: {Key}",
                key);

            await using var stream =
                file.OpenReadStream();

            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = key,
                InputStream = stream,
                ContentType = file.ContentType
            };

            await s3Client.PutObjectAsync(request);

            _logger.LogInformation(
                "S3 | Upload successful | Key: {Key}",
                key);

            return key;
        }
    }
}