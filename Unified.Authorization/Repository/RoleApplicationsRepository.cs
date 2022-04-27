using ApplicationPOCO;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unified.Authorization.Models;

namespace Unified.Authorization.Repository
{
    public class RoleApplicationsRepository : IRoleApplicationsRepository
    {
        private readonly IRoleRepository roleRepo;
        private readonly IHraApplicationRepository hraAppRepo;

        public RoleApplicationsRepository(IRoleRepository roleRepo, IHraApplicationRepository hraAppRepo)
        {
            this.roleRepo = roleRepo;
            this.hraAppRepo = hraAppRepo;
        }


        public async Task<Webresponse<Role_ApplicationsEdit>> GetRoleApplications(string RoleId)
        {

            var result = new Webresponse<Role_ApplicationsEdit>
            {
                status = APIStatus.processing,
                data = new Role_ApplicationsEdit()
            };

            try
            {
                var role = await roleRepo.GetById(RoleId);

                // Get all applications
                var AllApps = await hraAppRepo.GetAll();

                // Get the allowed applications Ids                                  
                var AllowedAppsIDs = await roleRepo.GetGrantedAppIds(RoleId);
                             
                if (role.status == APIStatus.success
                    && AllApps.status == APIStatus.success
                    && AllowedAppsIDs.status == APIStatus.success
                   )
                {
   
                    // Using the allowed apps Ids, get the allowed apps and denied apps
                    var AllowedApps = AllApps.data.Where(dt => AllowedAppsIDs.data.Contains(dt.ApplicationId)).ToList();
                    var DeniedApps = AllApps.data.Except(AllowedApps).ToList();

                    result.data = new Role_ApplicationsEdit
                    {
                        Role = role.data,
                        Members = AllowedApps,
                        NonMembers = DeniedApps
                    };

                    result.status = APIStatus.success;
                }
                else
                    throw new Exception("Something went wrong while trying to retrieve data for role applications");
            }
            catch (Exception ex)
            {
                Log.Error("RoleApplicationRepository.GetRoleApplication");
                result.status = APIStatus.error;
                result.message = ex.Message;
            }

            return result;

        }

    }
}
