using ApplicationPOCO;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Unified.Authorization.Models;

namespace Unified.Authorization.Repository
{
    public class RoleClaimsEditRepository : IRoleClaimsEditRepository
    {
        readonly IOptions<UserManagerConStrings> appsetting;

        readonly IRoleRepository roleRepo;
        readonly IHraApplicationRepository hraAppRepo;
        readonly IModuleRepository moduleRepo;
        readonly IClaimTypeRepository claimTypesRepo;


        public RoleClaimsEditRepository(IRoleRepository roleRepo, IHraApplicationRepository hraAppRepo,
            IModuleRepository moduleRepo, IClaimTypeRepository claimTypesRepo, IOptions<UserManagerConStrings> appsetting)
        {
            this.roleRepo = roleRepo;
            this.hraAppRepo = hraAppRepo;
            this.moduleRepo = moduleRepo;
            this.claimTypesRepo = claimTypesRepo;
            this.appsetting = appsetting;
        }

        private IDbConnection GetDbConnection()
        {
            return new SqlConnection(appsetting.Value.DBConnectionString);
        }

        public async Task<Webresponse<RoleClaimsEdit>> GetEditData(string RoleId, Guid ApplicationId, int? ModuleId)
        {

            var result = new Webresponse<RoleClaimsEdit>
            {
                status = APIStatus.processing,
                data = new RoleClaimsEdit()
            };

            try
            {
                var responseRole = await roleRepo.GetById(RoleId);
                var responseAccessibleApps = await roleRepo.GetGrantedAppIds(RoleId);
                var responseAllApps = await hraAppRepo.GetAll();
                var responseAppModules = await moduleRepo.Select(ApplicationId);

                // if ModuleId is null: Bing non-moduled claimTypes that belong to the selected application
                // else: bring module-related claimTypes
                var responseClaimTypes = await claimTypesRepo.Select(ApplicationId, ModuleId);
                var responseGivenClaims = await roleRepo.GrantedClaims(RoleId, ApplicationId, ModuleId);

                if
                (
                    responseRole.status == APIStatus.success
                    &&
                    responseAllApps.status == APIStatus.success
                    &&
                    responseAccessibleApps.status == APIStatus.success
                    &&
                    responseAppModules.status == APIStatus.success
                    &&
                    responseClaimTypes.status == APIStatus.success
                    &&
                    responseGivenClaims.status == APIStatus.success
                )
                {
                    var model = new RoleClaimsEdit
                    {
                        ApplicationId = ApplicationId,
                        ApplicationName = (responseAllApps.data.SingleOrDefault(dt => dt.ApplicationId == ApplicationId)?.ApplicationName),
                        Applications = responseAllApps.data,
                        AssessibleApps = responseAccessibleApps.data,
                        Modules = responseAppModules.data,
                        Role = responseRole.data,
                        ModuleId = ModuleId,
                        ClaimTypes = responseClaimTypes.data,
                        GivenClaims = responseGivenClaims.data
                    };

                    result.data = model;
                    result.status = APIStatus.success;
                }
                else
                    throw new Exception("Unable to query some data for editing role claims!");

            }
            catch (Exception ex)
            {
                string errorMsg = $"{GetType().Name} => {nameof(GetEditData)} \nError Message: {ex.Message}";
                Log.Error(errorMsg);
                result.status = APIStatus.error;
                result.message = errorMsg;
            }

            return result;
        }

        public async Task<WebresponseNoData> SaveClaims(RoleClaimsModify Model)
        {
            var result = new WebresponseNoData
            {
                status = APIStatus.processing
            };

            try
            {
                // Get the database connection
                using (var con = GetDbConnection())
                {
                    await con.ExecuteAsync("UpdateRole_Claims",
                    param: new
                    {
                        Model.RoleId,
                        Model.ApplicationId,
                        Model.ModuleId,
                        TVP_Grant = (new DataTableIntID(Model.GrantIds)).AsTableValuedParameter("dbo.IntID")
                    },
                    commandType: CommandType.StoredProcedure);
                }

                result.status = APIStatus.success;
            }
            catch (Exception ex)
            {
                string errorMsg = $"{GetType().Name} => {nameof(SaveClaims)} \nError Message: {ex.Message}";
                Log.Error(errorMsg);
                result.status = APIStatus.error;
                result.message = errorMsg;
            }

            return result;
        }


    }
}
