using Apex_Website_API.Model;
using Apex_Website_API.Repositories.Interfaces;
using Apex_Website_API.Services.Interfaces;

namespace Apex_Website_API.Services.Implementations
{
    public class ContactService : IContactService
    {
        private readonly IContactRepository _contactRepository;
        private readonly ILogger<ContactService> _logger;

        public ContactService(
            IContactRepository contactRepository,ILogger<ContactService> logger)
        {
            _contactRepository = contactRepository;
            _logger = logger;
        }

        public async Task<Guid> SaveContactAsync(Contact contact,string requestJson)
        {
            var responseJson = """
            {
                "success": true,
                "message": "Contact information saved successfully."
            }
            """;
            //_logger.LogInformation("CONTACT SERVICE | Save started");
            //var contactId = await _contactRepository.SaveContactAsync(contact,requestJson,responseJson);
            //_logger.LogInformation("CONTACT SERVICE | Save completed");

            //return contactId;
            try
            {
                _logger.LogInformation("CONTACT SERVICE | Save started");

                var contactId =await _contactRepository.SaveContactAsync(contact,requestJson,responseJson);

                _logger.LogInformation("CONTACT SERVICE | Save completed | ContactId: {ContactId}",contactId);

                return contactId;
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex,"CONTACT SERVICE | Validation error");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"CONTACT SERVICE | Save failed");
                throw;
            }
        }
    }
}