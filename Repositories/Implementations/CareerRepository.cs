using Apex_Website_API.Model;
using Apex_Website_API.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Apex_Website_API.Repositories.Implementations
{
    public class CareerRepository : ICareerRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<CareerRepository> _logger;

        public CareerRepository(IConfiguration configuration, ILogger<CareerRepository> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<Guid> SaveCareerAsync(Career career, string requestJson, string responseJson,string resumeBase64)
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");

                await using var connection = new SqlConnection(connectionString);

                await connection.OpenAsync();
                _logger.LogInformation("CAREER REPOSITORY | Database connection opened");
                // Convert Resume File to Base64

                _logger.LogInformation("CAREER REPOSITORY | Database insert started");
                await using var command = new SqlCommand("dbo.Sp_Save_CareerInformation", connection);

                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@Name", SqlDbType.NVarChar, 150).Value = career.FullName;
                command.Parameters.Add("@Email", SqlDbType.NVarChar, 200).Value = career.EmailId;
                command.Parameters.Add("@Phone", SqlDbType.NVarChar, 30).Value = career.PhoneNumber;
                command.Parameters.Add("@Position", SqlDbType.NVarChar, 150).Value = career.ApplyForPosition;
                command.Parameters.Add("@Experience", SqlDbType.NVarChar, 100).Value = (object?)career.Experience ?? DBNull.Value;
                // Resume File Name
                command.Parameters.Add("@ResumeFileName", SqlDbType.NVarChar,250).Value =(object?)career.UploadResume?.FileName ??DBNull.Value;
                // Resume Base64
                command.Parameters.Add("@ResumeContent",SqlDbType.NVarChar,-1).Value =(object?)resumeBase64 ??DBNull.Value;
                // Resume Content Type
                command.Parameters.Add("@ResumeContentType",SqlDbType.NVarChar,100).Value =(object?)career.UploadResume?.ContentType ??DBNull.Value;
                command.Parameters.Add("@Message", SqlDbType.NVarChar, -1).Value = (object?)career.Message ?? DBNull.Value;
                command.Parameters.Add("@CareerInformation_Request", SqlDbType.NVarChar, -1).Value = requestJson;
                command.Parameters.Add("@CareerInformation_Reponse", SqlDbType.NVarChar, -1).Value = (object?)responseJson ?? DBNull.Value;
                _logger.LogInformation("CAREER REPOSITORY | Stored procedure execution started");

                var result = await command.ExecuteScalarAsync();

                var careerId = (Guid)result!;

                _logger.LogInformation("CAREER REPOSITORY | Stored procedure execution completed");
                _logger.LogInformation("CAREER REPOSITORY | Database insert successful | CareerId: {CareerId}", careerId);

                return careerId;
                //return (Guid)result!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CAREER REPOSITORY | Database insertion failed");
                throw;
            }
        }
    }
}