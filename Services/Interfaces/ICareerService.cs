using Apex_Website_API.Model;

namespace Apex_Website_API.Services.Interfaces
{
    public interface ICareerService
    {
        Task<Guid> SaveCareerAsync(Career career, string requestjson);        
    }
}
