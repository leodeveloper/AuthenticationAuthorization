using ApplicationPOCO;
using ApplicationPOCO.LookDbsModel;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unified.Authorization.Models;

namespace Unified.Authorization.Repository
{
    public interface IUsersEditRepository
    {
        Task<Webresponse<UsersEdit>> Get(string UserId, string SearchText, int UserPageNo, int UserPageSize, int RolePageNo, int RolePageSize);
        
        Task<Webresponse<IdentityResult>> Lock(string UserId, bool Locked);
        Task<WebresponseNoData> Save(string UserId, IList<string> GivenRoleIds);
        
        Task<Webresponse<IList<HraApplications>>> GetGrantedApps(string UserId);
        Task<Webresponse<IList<Module>>> GetGrantedModules(string UserId, Guid? ApplicationId = null);
        Task<Webresponse<IList<ClaimType>>> GetGrantedClaims(string UserId);

        Task<Webresponse<List<UserInfo>>> GetUsersByModuleClaims(GetUsersByModuleClaimsParam param);
        
        Task<Webresponse<List<UserInfo>>> GetUsersByApplications(Guid applicationId);


        /// <summary>
        /// For example: getting roles for (JobApplication.Id, T0144). 
        /// This will be to get the transisions of a user using Role_Transitions table
        /// </summary>
        /// <param name="ApplicationId"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        Task<Webresponse<List<string>>> GetRoleIDsByApplicationUser(Guid ApplicationId, string UserId);


        Task<Webresponse<List<string>>> GetUsersEmails(string[] userIDs);


        Task<Webresponse<IList<ApplicationModules>>> GetUserApplicationModules(string UserId);
        Task<Webresponse<IList<RoleClaims>>> GetUserRolesAndClaims(string UserId, string ApplicationId, int? ModuleId = null);
        Task<Webresponse<IList<Module>>> IsUserAccessOfThisModules(string UserId, int ModuleId);

    }
}