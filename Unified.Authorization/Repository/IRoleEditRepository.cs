using ApplicationPOCO;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Unified.Authorization.Models;

namespace Unified.Authorization.Repository
{
    public interface IRoleEditRepository
    {
        Task<Webresponse<RoleEdit>> GetEditData(string RoleId);
        Task<Webresponse<IdentityResult>> Update(RoleModify Model);
    }
}