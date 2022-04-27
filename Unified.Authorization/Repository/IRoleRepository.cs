using ApplicationPOCO;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unified.Authorization.Models;

namespace Unified.Authorization.Repository
{
    public interface IRoleRepository
    {
        Task<Webresponse<IdentityResult>> Add(ApplicationRole Identity);
        Task<Webresponse<IdentityResult>> Disable(string RoleId, bool Disabled);
        Task<Webresponse<IdentityResult>> Update(RoleModify Model, UserManager<ApplicationUser> userManager);

        Task<WebresponsePaging<IList<ApplicationRole>>> GetAll(int Page, int Count, bool? Disabled);
        Task<Webresponse<IList<ApplicationRole>>> GetRolesByUserId(string UserId);
        Task<Webresponse<IList<string>>> GetRoleIdsForUser(string UserId);

        Task<Webresponse<ApplicationRole>> GetById(string RoleId);
        Task<Webresponse<List<Guid>>> GetGrantedAppIds(string RoleId);
        Task<Webresponse<List<ClaimType>>> GrantedClaims(string RoleId, Guid? ApplicationId, int? ModuleId = null);
        

    }
}