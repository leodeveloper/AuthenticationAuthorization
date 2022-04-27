using ApplicationPOCO;
using System.Threading.Tasks;
using Unified.Authorization.Models;

namespace Unified.Authorization.Repository
{
    public interface IRoleApplicationsRepository
    {
        Task<Webresponse<Role_ApplicationsEdit>> GetRoleApplications(string RoleId);
    }
}