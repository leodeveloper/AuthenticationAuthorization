using ApplicationPOCO;
using System;
using System.Threading.Tasks;
using Unified.Authorization.Models;

namespace Unified.Authorization.Repository
{
    public interface IRole_App_ClaimsRepository
    {
        Task<Webresponse<Role_App_ClaimsEdit>> Get(Guid ApplicationId, string RoleId);
        Task<WebresponseNoData> Save(Role_App_ClaimsModify Model);
    }
}