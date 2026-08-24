using Apex_Website_API.Model;

namespace Apex_Website_API.Repositories.Interfaces
{
    public interface IContactRepository
    {
        Task<Guid> SaveContactAsync(Contact contact,string requestJson,string responseJson);
    }
}