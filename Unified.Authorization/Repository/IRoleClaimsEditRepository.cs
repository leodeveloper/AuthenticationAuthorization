using ApplicationPOCO;
using System;
using System.Threading.Tasks;
using Unified.Authorization.Models;

namespace Unified.Authorization.Repository
{
    public interface IRoleClaimsEditRepository
    {
        Task<Webresponse<RoleClaimsEdit>> GetEditData(string RoleId, Guid ApplicationId, int? ModuleId);
        Task<WebresponseNoData> SaveClaims(RoleClaimsModify Model);
    }
}