using Apex_Website_API.Model;

namespace Apex_Website_API.Services.Interfaces
{
    public interface IContactService
    {
        Task<Guid> SaveContactAsync(Contact contact,string requestJson);
    }
}
