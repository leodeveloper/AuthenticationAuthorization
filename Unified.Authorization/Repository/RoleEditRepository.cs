using ApplicationPOCO;
using Dapper;
using Microsoft.AspNetCore.Identity;
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
    public class RoleEditRepository : IRoleEditRepository
    {
        readonly RoleManager<ApplicationRole> roleManager;
        readonly UserManager<ApplicationUser> userManager;
        readonly IOptions<UserManagerConStrings> appsetting;

        public RoleEditRepository(RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager, IOptions<UserManagerConStrings> appsetting)
        {            
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.appsetting = appsetting;
        }

        private IDbConnection GetDbConnection()
        {
            return new SqlConnection(appsetting.Value.DBConnectionString);
        }

        public async Task<Webresponse<RoleEdit>> GetEditData(string RoleId)
        {         
            var result = new Webresponse<RoleEdit>
            {
                status = APIStatus.processing,
                data = new RoleEdit()
            };

            try
            {
                var role = await roleManager.FindByIdAsync(RoleId);

                if (role != null)
                {
                    result.data.Role = role;

                    // Get role members

                    using (var con = GetDbConnection())
                    {
                        var multiReader = await con.QueryMultipleAsync("GetRoleUsers", param: new { RoleId }, 
                            commandType: CommandType.StoredProcedure);

                        result.data.Members = (await multiReader.ReadAsync<ApplicationUser>()).ToList();
                        result.data.NonMembers = (await multiReader.ReadAsync<ApplicationUser>()).ToList();
                    }
                    
                    result.status = APIStatus.success;
                }
                else
                {
                    result.status = APIStatus.error;
                    result.message = "Cannot find the role to edit!";
                }
            }
            catch(Exception ex)
            {
                Log.Error($"{GetType().Name}:{nameof(GetEditData)}\nError message => {ex.Message}");
                result.message = ex.Message;
                result.status = APIStatus.error;
            }

            return result;
        }

        public async Task<Webresponse<IdentityResult>> Update(RoleModify Model)
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
                Log.Error($"{GetType().Name}:{nameof(Update)}\nError message => {ex.Message}");
                result.message = ex.Message;
                result.status = APIStatus.error;
            }

            return result;

        }


    }
}
