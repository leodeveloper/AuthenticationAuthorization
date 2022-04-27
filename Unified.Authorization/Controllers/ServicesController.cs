using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationPOCO;
using ApplicationPOCO.LookDbsModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unified.Authorization.Models;
using Unified.Authorization.Repository;

namespace Unified.Authorization.Controllers
{
    [AllowAnonymous]
    [Route("[controller]/[action]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {

        readonly IUsersEditRepository usersEditRepo;
        readonly IModuleRepository moduleRepository;
        readonly IRoleRepository roleRepository;

        public ServicesController(IUsersEditRepository usersEditRepo, IModuleRepository moduleRepository, IRoleRepository roleRepository)
        {
            this.usersEditRepo = usersEditRepo;
            this.moduleRepository = moduleRepository;
            this.roleRepository = roleRepository;
        }


        /// <summary>
        /// The two parameters (Modules, ClaimTypes) are completely separate and belong to two different stored procedures
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<Webresponse<List<UserInfo>>> GetUsersByModuleClaims(GetUsersByModuleClaimsParam param)
        {

            //------------------------------
            // Testing:

            //param.ApplicationId = new Guid("7A7D333D-B4E7-4C0D-B81F-840A3A0149A4");

            //param.ClaimTypes = new List<int> { 8, 9 };
            //param.Modules = new List<int> { 3 };

            //------------------------------

            return await usersEditRepo.GetUsersByModuleClaims(param);
        }

        [HttpGet]
        public async Task<Webresponse<List<UserInfo>>> GetUsersByApplication(Guid applicationId)
        {
            return await usersEditRepo.GetUsersByApplications(applicationId);
        }

        [HttpGet]
        public async Task<Webresponse<List<string>>> GetRoleIDsByApplicationUser(Guid ApplicationId, string UserId)
        {
            return await usersEditRepo.GetRoleIDsByApplicationUser(ApplicationId, UserId);
        }

        [HttpGet]
        public async Task<Webresponse<IList<ApplicationModules>>> GetUserApplicationModules(string UserId)
        {
            return await usersEditRepo.GetUserApplicationModules(UserId);
        }

        /// <summary>
        /// this end point use in the jobapplication
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<Webresponse<IList<Module>>> GetModules()
        {
            return await moduleRepository.Select(null);
        }

        /// <summary>
        /// this end point use in the jobapplication
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<WebresponsePaging<IList<ApplicationRole>>> GetRoles()
        {
            return await roleRepository.GetAll(1, 10000, false);
        }

        [HttpGet]
        public async Task<Webresponse<IList<ApplicationRole>>> GetRolesByUserId(string userId)
        {
            return await roleRepository.GetRolesByUserId(userId);
        }


        [HttpGet]
        public async Task<Webresponse<IList<UserRoles>>> GetUserRolesAndClaims(string UserId, string ApplicationId, int? ModuleId = null)
        {
            try
            {
                var userCliams = await usersEditRepo.GetUserRolesAndClaims(UserId, ApplicationId, ModuleId);
                IList<UserRoles> userRoles = Convert_RoleClaims_into_UserModule.Convert(userCliams.data);
                return new Webresponse<IList<UserRoles>> { data = userRoles, status = APIStatus.success };
            }
            catch (Exception ex)
            {
                return new Webresponse<IList<UserRoles>> { status = APIStatus.error, message = ex.Message };
            }
        }


        [HttpPost]
        public async Task<Webresponse<List<string>>> GetUsersEmails(string[] UserIDs)
        {
            return await usersEditRepo.GetUsersEmails(UserIDs);
        }

        [HttpGet]
        public async Task<Webresponse<IList<Module>>> IsUserHasAccessRight(string UserId, int ModuleId)
        {
            
           // string user = User.Claims?.FirstOrDefault(x => x.Type.Equals("UserId", StringComparison.OrdinalIgnoreCase))?.Value;
            return await usersEditRepo.IsUserAccessOfThisModules(UserId, ModuleId);

        }


    }
}
