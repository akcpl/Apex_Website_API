using Apex_Website_API.Model;
using Apex_Website_API.Repositories.Interfaces;
using Apex_Website_API.Services.Implementations;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Apex_Website_API.Repositories.Implementations
{
    public class ContactRepository : IContactRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ContactRepository> _logger;

        public ContactRepository(IConfiguration configuration,ILogger<ContactRepository> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<Guid> SaveContactAsync(Contact contact,string requestJson,string responseJson)
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                await using var connection = new SqlConnection(connectionString);

                await connection.OpenAsync();
                _logger.LogInformation("CONTACT REPOSITORY | Database connection opened");

                _logger.LogInformation("CONTACT REPOSITORY | Database insert started");
                await using var command = new SqlCommand("dbo.Sp_Save_ContactInformation", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@Name", SqlDbType.NVarChar, 150).Value = contact.Name;
                command.Parameters.Add("@Email", SqlDbType.NVarChar, 200).Value = contact.Email;
                command.Parameters.Add("@Phone", SqlDbType.NVarChar, 30).Value = contact.Phone;
                command.Parameters.Add("@City", SqlDbType.NVarChar, 150).Value = contact.City;
                command.Parameters.Add("@Query", SqlDbType.NVarChar, 250).Value = (object?)contact.Query ?? DBNull.Value;
                command.Parameters.Add("@Contact_Request", SqlDbType.NVarChar, -1).Value = requestJson;
                command.Parameters.Add("@Contact_Response", SqlDbType.NVarChar, -1).Value = responseJson;
                _logger.LogInformation("CONTACT REPOSITORY | Stored procedure execution started");

                var result = await command.ExecuteScalarAsync();
                var contactId = (Guid)result!;

                _logger.LogInformation("CONTACT REPOSITORY | Stored procedure execution completed");
                _logger.LogInformation("CONTACT REPOSITORY | Database insert successful | ContactId: {ContactId}", contactId);

                return contactId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"CONTACT REPOSITORY | Database insertion failed");
                throw;
            }
        }
    }
}