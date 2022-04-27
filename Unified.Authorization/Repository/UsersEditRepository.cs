using ApplicationPOCO;
using ApplicationPOCO.LookDbsModel;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Unified.Authorization.Models;

namespace Unified.Authorization.Repository
{
    public class UsersEditRepository : IUsersEditRepository
    {
        // for the database connection
        readonly IOptions<UserManagerConStrings> appsetting;
        
        // for reading contact information
        readonly IOptions<EntitiesApi> entitiesApi;

        // for getting the roles through sp paging 
        readonly IRoleRepository roleRepo;

        // for locking user
        readonly UserManager<ApplicationUser> userManager;

        public UsersEditRepository(IOptions<UserManagerConStrings> appsetting, IOptions<EntitiesApi> entitiesApi, //RoleManager<ApplicationRole> roleManager,
             IRoleRepository roleRepo, UserManager<ApplicationUser> userManager)
        {
            this.appsetting = appsetting;
            this.entitiesApi = entitiesApi;

            this.roleRepo = roleRepo;
            this.userManager = userManager;            
        }

        private IDbConnection GetDbConnection()
        {
            return new SqlConnection(appsetting.Value.DBConnectionString);
        }

        /// <summary>
        /// Gets users editing info (with or w/o selected user)
        /// </summary>
        /// <param name="UserId">The selected user</param>
        /// <returns></returns>
        public async Task<Webresponse<UsersEdit>> Get(string UserId, string SearchText,
            int UserPageNo, int UserPageSize, int RolePageNo, int RolePageSize)
        {
            var result = new Webresponse<UsersEdit>
            {
                data = new UsersEdit { UserId = UserId },
                status = APIStatus.processing
            };

            try
            {                                
                // 1: Get users
                result.data.Users = await User_Select(SearchText, UserPageNo, UserPageSize);

                // 2: Get enabled roles only
                result.data.Roles = await roleRepo.GetAll(RolePageNo, RolePageSize, false);
                                                               
                if (!string.IsNullOrEmpty(UserId))
                {
                    var user = result.data.Users.data.SingleOrDefault(dt => dt.Id == UserId);
                    if (user != null)
                    {
                        // 3: Get ContactInfo
                        UriBuilder builder = new UriBuilder(entitiesApi.Value.GetContactbyEmail);
                        builder.Query = $"contactEmail={user.Email}";

                        using (HttpClient _httpClient = new HttpClient())
                        {                            
                            var response = _httpClient.GetAsync(builder.Uri).Result;
                            if (response.IsSuccessStatusCode)
                            {                             
                                var contact = await response.Content.ReadFromJsonAsync<Webresponse<ContactInfo>>();
                                result.data.ContactInfo = contact.data;                              
                            }
                        }

                        // 4: Get Given Roles
                        var givenRolesResponse = await roleRepo.GetRoleIdsForUser(UserId);
                        if (givenRolesResponse.status == APIStatus.success)
                            result.data.GivenRoleIds = givenRolesResponse.data;
                        else
                            throw new Exception(givenRolesResponse.message);

                        //5: Get the granted applications
                        var grantedAppsResponse = await GetGrantedApps(UserId);
                        if (grantedAppsResponse.status == APIStatus.success)
                            result.data.GrantedApplications = grantedAppsResponse;
                        else
                            throw new Exception(grantedAppsResponse.message);

                        //6: Get the granted modules
                        var grantedModulesResponse = await GetGrantedModules(UserId);
                        if (grantedModulesResponse.status == APIStatus.success)
                            result.data.GrantedModules = grantedModulesResponse;
                        else
                            throw new Exception(grantedModulesResponse.message);

                        //7: Get the granted claims
                        var grantedClaimsResponse = await GetGrantedClaims(UserId);
                        if (grantedClaimsResponse.status == APIStatus.success)
                            result.data.GrantedClaims = grantedClaimsResponse;
                        else
                            throw new Exception(grantedClaimsResponse.message);
                    }
                }

                result.status = APIStatus.success;
            }
            catch (Exception ex)
            {
                Log.Error($"{nameof(UsersEditRepository)}:{nameof(Get)}:: Error message => {ex.Message}");
                result.message = ex.Message;
                result.status = APIStatus.error;
            }

            return result;

        }
        
        private async Task<WebresponsePaging<IList<ApplicationUser>>> User_Select(string Text, int Page, int Count)
        {
            var response = new WebresponsePaging<IList<ApplicationUser>>
            { 
                status = APIStatus.processing,
                page = Page,
                pageSize = Count
            };

            try
            {
                using (var con = GetDbConnection())
                {
                    var p = new DynamicParameters();
                    p.Add("Text", Text); p.Add("Page", Page); p.Add("Count", Count);
                    p.Add("TotalCount", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    response.data = (await con.QueryAsync<ApplicationUser>("User_Select", param: p, 
                        commandType: CommandType.StoredProcedure)).ToList();
                                        
                    response.totalrecords = p.Get<int>("TotalCount");                    
                }

                response.status = APIStatus.success;

            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                response.message = ex.Message;
                response.status = APIStatus.error;
            }

            return response;
        }

        
        /// <summary>
        /// Saves and then calls the Repository.Get
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="GivenRoleIds"></param>
        /// <returns></returns>        
        public async Task<WebresponseNoData> Save(string UserId, IList<string> GivenRoleIds)
        {
            var result = new WebresponseNoData
            {                
                status = APIStatus.processing
            };
           
            try
            {
                using (var con = GetDbConnection())
                {
                    var res = await con.ExecuteAsync("UpdateUser_Roles", 
                        param: new { UserId, TVP_GivenRoleIds = new DataTableStrID(GivenRoleIds).
                            AsTableValuedParameter("dbo.IdentityID") },
                        commandType: CommandType.StoredProcedure);                    
                }

                result.status = APIStatus.success;

            }
            catch (Exception ex)
            {
                Log.Error($"{nameof(UsersEditRepository)}:{nameof(Save)}:: Error message => {ex.Message}");
                result.message = ex.Message;
                result.status = APIStatus.error;
            }

            return result;
        }

        /// <summary>
        /// Locks out the selected user
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="Locked"></param>
        /// <returns></returns>
        public async Task<Webresponse<IdentityResult>> Lock(string UserId, bool Locked)
        {

            var result = new Webresponse<IdentityResult>
            {
                data = new IdentityResult(),
                status = APIStatus.processing
            };

            try
            {
                ApplicationUser user = await userManager.FindByIdAsync(UserId);

                if (user != null)
                {                 
                    result.data = await userManager.SetLockoutEnabledAsync(user, Locked);

                    DateTimeOffset? dateOffset = null; if (Locked) dateOffset = new DateTimeOffset(DateTime.Now.AddYears(5));
                    await userManager.SetLockoutEndDateAsync(user, dateOffset);
                }
                result.status = APIStatus.success;
            }
            catch (Exception ex)
            {
                Log.Error($"{nameof(UsersEditRepository)}:{nameof(Lock)}:: Error message => {ex.Message}");
                result.message = ex.Message;
                result.status = APIStatus.error;
            }

            return result;
        }


        #region << Granted stuff >>

        /// <summary>
        /// Get the granted applications
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public async Task<Webresponse<IList<HraApplications>>> GetGrantedApps(string UserId)
        {
            var result = new Webresponse<IList<HraApplications>>
            {                
                status = APIStatus.processing,
                data = new List<HraApplications>()
            };

            try
            {
                using (var con = GetDbConnection())
                {
                    var res = await con.QueryAsync<HraApplications>("GetUserApplications", 
                        param: new { UserId }, commandType: CommandType.StoredProcedure);

                    result.data = res.ToList();
                }

                result.status = APIStatus.success;
            }
            catch (Exception ex)
            {
                Log.Error($"{GetType().Name}:{nameof(GetGrantedApps)}\nError message => {ex.Message}");
                result.message = ex.Message;
                result.status = APIStatus.error;
            }

            return result;

        }

        /// <summary>
        /// Get the granted modules per application
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="ApplicationId">null to bring for all applications</param>
        /// <returns></returns>
        public async Task<Webresponse<IList<Module>>> GetGrantedModules(string UserId, Guid? ApplicationId = null)
        {
            var result = new Webresponse<IList<Module>>
            {
                status = APIStatus.processing,
                data = new List<Module>()
            };

            try
            {
                using (var con = GetDbConnection())
                {
                    var res = await con.QueryAsync<Module>("GetUserModules",
                        param: new { UserId, ApplicationId }, commandType: CommandType.StoredProcedure);
                    
                    result.data = res.ToList();
                }

                result.status = APIStatus.success;
            }
            catch (Exception ex)
            {
                Log.Error($"{GetType().Name}:{nameof(GetGrantedModules)}\nError message => {ex.Message}");
                result.message = ex.Message;
                result.status = APIStatus.error;
            }

            return result;
        }

        /// <summary>
        /// Check weather the user has the access right to this module
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="ModuleId">null to bring for all applications</param>
        /// <returns></returns>
        public async Task<Webresponse<IList<Module>>> IsUserAccessOfThisModules(string UserId, int ModuleId)
        {
          //  userManager.getuser
          //  string user = User.Claims?.FirstOrDefault(x => x.Type.Equals("UserId", StringComparison.OrdinalIgnoreCase))?.Value;
            var result = new Webresponse<IList<Module>>
            {
                status = APIStatus.processing,
                data =new List<Module>()
            };

            try
            {
                using (var con = GetDbConnection())
                {
                    var res = await con.QueryAsync<Module>("GetUserModulesByModuleId",
                        param: new { UserId, ModuleId }, commandType: CommandType.StoredProcedure);

                    if (res.Count() > 0)
                        result.data = res.ToList();
                }

                result.status = APIStatus.success;
            }
            catch (Exception ex)
            {
                Log.Error($"{GetType().Name}:{nameof(IsUserAccessOfThisModules)}\nError message => {ex.Message}");
                result.message = ex.Message;
                result.status = APIStatus.error;
            }

            return result;
        }


        /// <summary>
        /// Get the granted claims per user only
        /// </summary>
        /// <param name="UserId"></param>        
        /// <returns></returns>
        public async Task<Webresponse<IList<ClaimType>>> GetGrantedClaims(string UserId)
        {
            var result = new Webresponse<IList<ClaimType>>
            {
                status = APIStatus.processing,
                data = new List<ClaimType>()
            };

            try
            {
                using (var con = GetDbConnection())
                {
                    var res = await con.QueryAsync<ClaimType>("GetUserClaims",
                        param: new { UserId }, commandType: CommandType.StoredProcedure);

                    result.data = res.ToList();
                }

                result.status = APIStatus.success;
            }
            catch (Exception ex)
            {
                Log.Error($"{GetType().Name}:{nameof(GetGrantedClaims)}\nError message => {ex.Message}");
                result.message = ex.Message;
                result.status = APIStatus.error;
            }

            return result;
        }

        public async Task<Webresponse<List<UserInfo>>> GetUsersByModuleClaims(GetUsersByModuleClaimsParam param)
        {

            var result = new Webresponse<List<UserInfo>>
            {
                status = APIStatus.processing,
                data = new List<UserInfo>()
            };

            try
            {
                using (var con = GetDbConnection())
                {
                    //-----------------------------------
                    //1: Get users for modules w/o claims

                    //var listModules = listModuleClaims.Where(d => d.ClaimTypes.Count < 1).Select(d => d.ModuleId).ToList();

                    var res1 = await con.QueryAsync<UserInfo>("GetUsersByModules",
                        param: new 
                        {
                            param.ApplicationId,
                            Modules = (new DataTableIntID(param.Modules)).AsTableValuedParameter("dbo.IntID")
                        }, 
                        commandType: CommandType.StoredProcedure);

                    //-------------------------------
                    //2: Get users for modules claims 

                    //var listClaimTypes = listModuleClaims.Where(d => d.ClaimTypes.Count > 0).SelectMany(d => d.ClaimTypes).ToList();

                    var res2 = await con.QueryAsync<UserInfo>("GetUsersByClaimTypes",
                        param: new
                        {
                            param.ApplicationId,
                            ClaimTypes = (new DataTableIntID(param.ClaimTypes)).AsTableValuedParameter("dbo.IntID")
                        },
                        commandType: CommandType.StoredProcedure);

                    //----------------------------------
                    // 3: Merge the results from 1 and 2

                    result.data.AddRange(res1);
                    result.data.AddRange(res2);
                }

                result.status = APIStatus.success;

            }
            catch (Exception ex)
            {
                Log.Error($"{GetType().Name}:{nameof(GetUsersByModuleClaims)}\nError message => {ex.Message}");
                result.message = ex.Message;
                result.status = APIStatus.error;
            }

            return result;

        }

        public async Task<Webresponse<IList<ApplicationModules>>> GetUserApplicationModules(string UserId)
        {

            var result = new Webresponse<IList<ApplicationModules>>
            {
                status = APIStatus.processing,
                data = new List<ApplicationModules>()
            };

            try
            {
                using (var con = GetDbConnection())
                {

                    var res = await con.QueryAsync<ApplicationModules>("GetUserAppsAndModules",
                        param: new
                        {
                            UserId,
                            ApplicationId = (Guid?)null
                        },
                        commandType: CommandType.StoredProcedure);

                    result.data = res.ToList();
                }

                result.status = APIStatus.success;

            }
            catch (Exception ex)
            {
                Log.Error($"{GetType().Name}:{nameof(GetUserApplicationModules)}\nError message => {ex.Message}");
                result.message = ex.Message;
                result.status = APIStatus.error;
            }

            return result;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public async Task<Webresponse<List<UserInfo>>> GetUsersByApplications(Guid applicationId)
        {

            var result = new Webresponse<List<UserInfo>>
            {
                status = APIStatus.processing,
                data = new List<UserInfo>()
            };

            try
            {
                using (var con = GetDbConnection())
                {
                                        
                    var res = await con.QueryAsync<UserInfo>("GetUsersByApplications",
                        param: new
                        {
                            ApplicationId = applicationId
                        },
                        commandType: CommandType.StoredProcedure);
                                        
                    result.data = res.ToList();                    
                }

                result.status = APIStatus.success;

            }
            catch (Exception ex)
            {
                Log.Error($"{GetType().Name}:{nameof(GetUsersByApplications)}\nError message => {ex.Message}");
                result.message = ex.Message;
                result.status = APIStatus.error;
            }

            return result;

        }



        /// <summary>
        /// GetRoleIDsByApplicationUser
        ///     @ApplicationId UniqueIdentifier,
        ///     @UserId nvarchar(256)
        /// </summary>
        /// <param name="ApplicationId"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public async Task<Webresponse<List<string>>> GetRoleIDsByApplicationUser(Guid ApplicationId, string UserId)
        {

            var result = new Webresponse<List<string>>
            {
                status = APIStatus.processing,
                data = new List<string>()
            };

            try
            {
                using (var con = GetDbConnection())
                {

                    var res = await con.QueryAsync<string>("GetRoleIDsByApplicationUser",
                        param: new
                        {
                            ApplicationId = ApplicationId,
                            UserId = UserId
                        },
                        commandType: CommandType.StoredProcedure);

                    result.data = res.ToList();
                }

                result.status = APIStatus.success;

            }
            catch (Exception ex)
            {
                Log.Error($"{GetType().Name}:{nameof(GetRoleIDsByApplicationUser)}\nError message => {ex.Message}");
                result.message = ex.Message;
                result.status = APIStatus.error;
            }

            return result;

        }


        public async Task<Webresponse<List<string>>> GetUsersEmails(string[] userIDs)
        {
            var result = new Webresponse<List<string>>
            {
                status = APIStatus.processing,
                data = new List<string>()
            };

            try
            {
                string strUserIDs = "'" + string.Join("','", userIDs) + "'";
                    
                using (var con = GetDbConnection())
                {
                    string sql = $"Select Email from AspNetUsers where Id in ( {strUserIDs} )";
                    var res = await con.QueryAsync<string>(sql, commandType: CommandType.Text);

                    result.data = res.ToList();
                }

                result.status = APIStatus.success;

            }
            catch (Exception ex)
            {
                Log.Error($"{GetType().Name}:{nameof(GetUsersByApplications)}\nError message => {ex.Message}");
                result.message = ex.Message;
                result.status = APIStatus.error;
            }

            return result; 
        }


        public async Task<Webresponse<IList<RoleClaims>>> GetUserRolesAndClaims
            (string UserId, string ApplicationId, int? ModuleId = null)
        {

            var result = new Webresponse<IList<RoleClaims>>
            {
                status = APIStatus.processing,
                data = new List<RoleClaims>()
            };

            try
            {
                using (var con = GetDbConnection())
                {

                    var res = await con.QueryAsync<RoleClaims>("GetUserRolesAndClaims",
                        param: new { UserId, ApplicationId, ModuleId },
                        commandType: CommandType.StoredProcedure);

                    result.data = res.ToList();
                }

                result.status = APIStatus.success;

            }
            catch (Exception ex)
            {
                Log.Error($"{GetType().Name}:{ nameof(GetUserRolesAndClaims) }\nError message => {ex.Message}");
                result.message = ex.Message;
                result.status = APIStatus.error;
            }

            return result;

        }


        #endregion

    }

}
