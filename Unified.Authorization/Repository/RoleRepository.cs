using ApplicationPOCO;
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
using System.Threading.Tasks;
using Unified.Authorization.Models;

namespace Unified.Authorization.Repository
{
    public class RoleRepository : IRoleRepository
    {

        readonly IOptions<UserManagerConStrings> appsetting;
        readonly RoleManager<ApplicationRole> roleManager;        

        public RoleRepository(RoleManager<ApplicationRole> roleManager, 
            IOptions<UserManagerConStrings> appsetting)
        {
            this.appsetting = appsetting;            
            this.roleManager = roleManager;
        }

        private IDbConnection GetDbConnection()
        {
            return new SqlConnection(appsetting.Value.DBConnectionString);
        }

      
        /// <summary>
        /// Get roles for role editor
        /// </summary>
        /// <param name="Page"></param>
        /// <param name="Count"></param>
        /// <param name="Disabled"></param>
        /// <returns></returns>
        public async Task<WebresponsePaging<IList<ApplicationRole>>> GetAll(int Page, int Count, bool? Disabled)
        {
            var result = new WebresponsePaging<IList<ApplicationRole>>
            {
                data = new List<ApplicationRole>(),
                pageSize = Count, page = Page
            };

            try
            {
                using (var con = GetDbConnection())
                {
                    var p = new DynamicParameters();
                    p.Add("Page", Page); p.Add("Count", Count); p.Add("Disabled", Disabled);
                    p.Add("TotalCount", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    result.data = (await con.QueryAsync<ApplicationRole>("Role_Select", 
                        param: p, commandType:CommandType.StoredProcedure)).ToList();

                    result.totalrecords = p.Get<int>("TotalCount");                   
                }
                
                result.status = APIStatus.success;
            }
            catch (Exception ex)
            {
                Log.Error($"{GetType().Name}:{nameof(GetAll)}\nError message: {ex.Message}");
                result.message = ex.Message;
                result.status = APIStatus.error;
            }

            return result;

        }

        /// <summary>
        /// This method use in the job application work flow micor service
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public async Task<Webresponse<IList<ApplicationRole>>> GetRolesByUserId(string UserId)
        {
            var result = new Webresponse<IList<ApplicationRole>>
            {
                data = new List<ApplicationRole>()
            };

            try
            {
                using (var con = GetDbConnection())
                {
                    result.data = (await con.QueryAsync<ApplicationRole>("GetRolesByUserId",
                        param: new { UserId }, commandType: CommandType.StoredProcedure)).ToList();
                }

                result.status = APIStatus.success;
            }
            catch (Exception ex)
            {
                Log.Error($"{GetType().Name}:{nameof(GetAll)}\nError message: {ex.Message}");
                result.message = ex.Message;
                result.status = APIStatus.error;
            }
            return result;
        }

        /// <summary>
        /// Get roles by userId 
        /// </summary>
        /// <param name="Page"></param>
        /// <param name="Count"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public async Task<Webresponse<IList<string>>> GetRoleIdsForUser(string UserId)
        {
            var result = new Webresponse<IList<string>>
            {
                data = new List<string>()                
            };

            try
            {
                using (var con = GetDbConnection())
                {                    
                    result.data = (await con.QueryAsync<string>("GetRoleIdsForUser",
                        param: new { UserId }, commandType: CommandType.StoredProcedure)).ToList();
                }

                result.status = APIStatus.success;
            }
            catch (Exception ex)
            {
                Log.Error($"{GetType().Name}:{nameof(GetAll)}\nError message: {ex.Message}");
                result.message = ex.Message;
                result.status = APIStatus.error;
            }

            return result;
        }
        

        public async Task<Webresponse<ApplicationRole>> GetById(string RoleId)
        {
            var result = new Webresponse<ApplicationRole>
            {
                data = new ApplicationRole(),
                status = APIStatus.processing
            };

            try
            {
                result.data = await roleManager.FindByIdAsync(RoleId);
                result.status = APIStatus.success;
            }
            catch (Exception ex)
            {
                Log.Error("RoleRepository:GetById:: Error message => {0}", ex.Message);
                result.message = ex.Message;
                result.status = APIStatus.error;
            }

            return result;
        }

        /// <summary>
        /// For Create new db record
        /// </summary>
        /// <returns></returns>
        public async Task<Webresponse<IdentityResult>> Add(ApplicationRole Role)
        {
            var result = new Webresponse<IdentityResult>
            {
                data = new IdentityResult(),
                status = APIStatus.processing
            };

            try
            {
                if (!await roleManager.RoleExistsAsync(Role.Name))
                {
                    result.data = await roleManager.CreateAsync(Role);
                    result.status = APIStatus.success;
                }
                else
                    throw new Exception($"Role [{Role.Name}] already exists");
            }
            catch (Exception ex)
            {
                Log.Error($"{nameof(RoleRepository)}:{nameof(Add)}\nError message => {ex.Message}");
                result.message = ex.Message;
                result.status = APIStatus.error;
            }

            return result;

        }


        /// <summary>
        /// Private async method to update the name of the role
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Webresponse<IdentityResult>> Update(RoleModify Model, 
            UserManager<ApplicationUser> userManager)
        {
            var result = new Webresponse<IdentityResult>
            {
                data = new IdentityResult(),
                status = APIStatus.processing
            };

            try
            {
                var role = await roleManager.FindByIdAsync(Model.Id);
                if (role != null)
                {
                    role.Name = Model.Name;
                    role.ArName = Model.ArName;
                    role.Description = Model.Description;

                    result.data = await roleManager.UpdateAsync(role);

                    //-------------------------------------
                    // Save the users in the selected role

                    foreach (string userId in Model.AddIds ?? new string[] { })
                    {
                        ApplicationUser user = await userManager.FindByIdAsync(userId);

                        if (user != null)
                        {
                            var IdentityResult = await userManager.AddToRoleAsync(user, role.Name);
                            if (!IdentityResult.Succeeded)
                                throw new Exception("Cannot add user to the selected role!");

                        }
                    }
                    foreach (string userId in Model.DeleteIds ?? new string[] { })
                    {
                        ApplicationUser user = await userManager.FindByIdAsync(userId);
                        if (user != null)
                        {
                            var IdentityResult = await userManager.RemoveFromRoleAsync(user, role.Name);
                            if (!IdentityResult.Succeeded)
                            {
                                throw new Exception("Cannot remove user from the selected role!");
                            }
                        }
                    }

                    result.status = APIStatus.success;
                }
                else
                    throw new Exception("The selected role could not be found in the database!");
                
            }
            catch (Exception ex)
            {
                Log.Error($"{nameof(RoleRepository)}:{nameof(Update)}\nError message => {ex.Message}");
                result.message = ex.Message;
                result.status = APIStatus.error;
            }

            return result;

        }


        /// <summary>
        /// Update the Disabled field to the passed value
        /// </summary>
        /// <param name="RoleId"></param>
        /// <param name="Disabled"></param>
        /// <returns></returns>
        public async Task<Webresponse<IdentityResult>> Disable(string RoleId, bool Disabled)
        {

            var result = new Webresponse<IdentityResult>
            {
                data = new IdentityResult(),
                status = APIStatus.processing
            };

            try
            {
                ApplicationRole role = await roleManager.FindByIdAsync(RoleId);

                if (role != null)
                {
                    role.Disabled = Disabled;
                    result.data = await roleManager.UpdateAsync(role);
                }
                result.status = APIStatus.success;
            }
            catch (Exception ex)
            {
                Log.Error($"{nameof(RoleRepository)}:{nameof(Disable)}\nError message => {ex.Message}");
                result.message = ex.Message;
                result.status = APIStatus.error;
            }

            return result;
        }


        /// <summary>
        /// Returns the granted applications for the current role
        /// </summary>
        /// <param name="con"></param>
        /// <returns></returns>
        public async Task<Webresponse<List<Guid>>> GetGrantedAppIds(string RoleId)
        {
            var result = new Webresponse<List<Guid>>
            {
                status = APIStatus.processing,
                data = new List<Guid>()
            };

            try
            {
                using (var con = GetDbConnection())
                {
                    string query = $"GetRoleApplications @RoleId = '{RoleId}'";
                    result.data = (await con.QueryAsync<Guid>(query, commandType: CommandType.Text)).ToList();
                }
                result.status = APIStatus.success;
            }
            catch (Exception ex)
            {
                Log.Error($"{nameof(RoleRepository)}:{nameof(GetGrantedAppIds)}\nError message => {ex.Message}");
                result.message = ex.Message;
                result.status = APIStatus.error;
            }

            return result;

        }

        /// <summary>
        /// Returns the granted applications for the passed role
        /// </summary>
        /// <param name="RoleId"></param>
        /// <param name="ApplicationId"></param>
        /// <param name="ModuleId"></param>
        /// <param name="ApplyModuleIdNull">It means if (true) and ModuleId is null, apply the condition rather than ignoring it</param>
        /// <returns></returns>
        public async Task<Webresponse<List<ClaimType>>> GrantedClaims
            (string RoleId, Guid? ApplicationId, int? ModuleId = null)
        {
            var result = new Webresponse<List<ClaimType>>
            {
                status = APIStatus.processing,
                data = new List<ClaimType>()
            };

            try
            {
                using (var con = GetDbConnection())
                {                    
                    result.data = (await con.QueryAsync<ClaimType>("GetRoleClaims", 
                        param: new { RoleId, ApplicationId, ModuleId },
                        commandType: CommandType.StoredProcedure)).ToList();
                }
                result.status = APIStatus.success;
            }
            catch (Exception ex)
            {
                Log.Error($"{nameof(RoleRepository)}:{nameof(GrantedClaims)}\nError message => {ex.Message}");
                result.message = ex.Message;
                result.status = APIStatus.error;
            }

            return result;

        }




    }

}
