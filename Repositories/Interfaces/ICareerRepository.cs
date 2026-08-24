using Apex_Website_API.Model;

namespace Apex_Website_API.Repositories.Interfaces
{
    public interface ICareerRepository
    {
        //Task<Guid> SaveCareerAsync(Career career, string requestJson, string responseJson);
        Task<Guid> SaveCareerAsync(Career career,string requestJson,string responseJson,string resumeBase64);
    }
}
