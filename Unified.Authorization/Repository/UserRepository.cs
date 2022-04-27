using ApplicationPOCO;
using Microsoft.AspNetCore.Identity;

using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unified.Authorization.Models;

namespace Unified.Authorization.Repository
{
    public class UserRepository : IUserRepository
    {

        #region << Constructors >>

        public UserRepository(UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            //this.appsetting = appsetting;            
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        #endregion

        #region << Fields >>

        //readonly IOptions<UserManagerConStrings> appsetting;
        readonly UserManager<ApplicationUser> userManager;
        readonly RoleManager<ApplicationRole> roleManager;


        #endregion

        #region << Methods >>

        public Webresponse<IList<ApplicationUser>> GetAll()
        {
            var result = new Webresponse<IList<ApplicationUser>>
            {
                data = new List<ApplicationUser>(),
                status = APIStatus.processing
            };

            try
            {
                result.data = userManager.Users.ToList();
                result.status = APIStatus.success;
            }
            catch (Exception ex)
            {
                Log.Error("UserRepository:GetAll:: Error message => {0}", ex.Message);
                result.message = ex.Message;
                result.status = APIStatus.error;
            }

            return result;

        }

        public async Task<Webresponse<ApplicationUser>> GetById(string UserId)
        {
            var result = new Webresponse<ApplicationUser>
            {
                data = new ApplicationUser(),
                status = APIStatus.processing
            };

            try
            {
                result.data = await userManager.FindByIdAsync(UserId);
                result.status = APIStatus.success;
            }
            catch (Exception ex)
            {
                Log.Error("UserRepository:GetById:: Error message => {0}", ex.Message);
                result.message = ex.Message;
                result.status = APIStatus.error;
            }

            return result;
        }


        /// <summary>
        /// For Create new db record
        /// </summary>
        /// <returns></returns>
        public async Task<Webresponse<IdentityResult>> Add(ApplicationUser Identity)
        {
            var result = new Webresponse<IdentityResult>
            {
                data = new IdentityResult(),
                status = APIStatus.processing
            };

            try
            {
                result.data = await userManager.CreateAsync(Identity);
                result.status = APIStatus.success;
            }
            catch (Exception ex)
            {
                Log.Error("UserRepository:Add:: Error message => {0}", ex.Message);
                result.message = ex.Message;
                result.status = APIStatus.error;
            }

            return result;

        }


        /// <summary>
        /// Get the editing data for user editing
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public async Task<Webresponse<UserEdit>> GetEditingData(string UserId)
        {
            var result = new Webresponse<UserEdit>
            {
                data = new UserEdit(),
                status = APIStatus.processing
            };

            try
            {

                ApplicationUser user = await userManager.FindByIdAsync(UserId);

                List<ApplicationRole> members = new List<ApplicationRole>();
                List<ApplicationRole> nonMembers = new List<ApplicationRole>();

                var roles = roleManager.Roles.ToList();

                foreach (ApplicationRole role in roles)
                {
                    // Define the list which you want to add the role to
                    var list = await userManager.IsInRoleAsync(user, role.Name) ? members : nonMembers;
                    list.Add(role);
                }

                result.data = new UserEdit { User = user, Members = members, NonMembers = nonMembers };

            }
            catch (Exception ex)
            {
                Log.Error("UserRepository:GetEditingData:: Error message => {0}", ex.Message);
                result.message = ex.Message;
                result.status = APIStatus.error;
            }

            return result;
        }

        /// <summary>
        /// Submit the changes to the database
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<WebresponseNoData> Update(UserModify model)
        {
            var result = new WebresponseNoData
            {
                status = APIStatus.processing
            };

            try
            {
                ApplicationUser user = await userManager.FindByIdAsync(model.UserId);

                if (user != null)
                {
                    // update the user info, like the first name, last name, etc.
                    if (!(await userManager.UpdateAsync(user)).Succeeded)
                        throw new Exception($"Could not update user data [{model.UserId}]");

                    //-------------------------------------
                    // Save the users in the selected role

                    foreach (string roleId in model.AddIds ?? new string[] { })
                    {
                        // Get the role and then assign the user to it
                        var role = await roleManager.FindByIdAsync(roleId);
                        if (role != null)
                        {
                            if (!(await userManager.AddToRoleAsync(user, role.Name)).Succeeded)
                                throw new Exception($"Could not add the user to role [{role.Name}]");
                        }
                    }
                    foreach (string roleId in model.DeleteIds ?? new string[] { })
                    {
                        var role = await roleManager.FindByIdAsync(roleId);
                        if (role != null)
                        {
                            if (!(await userManager.RemoveFromRoleAsync(user, role.Name)).Succeeded)
                                throw new Exception($"Could not remove the user from role [{role.Name}]");
                        }
                    }

                    result.status = APIStatus.success;
                }
                else
                {
                    throw new Exception("The user could not be found!");
                }

            }
            catch (Exception ex)
            {
                result.status = APIStatus.error;
                result.message = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// Locks out the selected user
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="Disabled"></param>
        /// <returns></returns>
        public async Task<Webresponse<IdentityResult>> Lock(string UserId, bool Disabled)
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
                    //user.Disabled = Disabled;
                    result.data = await userManager.SetLockoutEnabledAsync(user, Disabled); //UpdateAsync(user);
                }
                result.status = APIStatus.success;
            }
            catch (Exception ex)
            {
                Log.Error("UserRepository:Disable:: Error message => {0}", ex.Message);
                result.message = ex.Message;
                result.status = APIStatus.error;
            }

            return result;
        }

        public async Task<Webresponse<bool>> IsPartOfRole(ApplicationUser User, string RoleName)
        {

            var result = new Webresponse<bool>
            {
                status = APIStatus.processing
            };

            try
            {
                bool res = await userManager.IsInRoleAsync(User, RoleName);
                result.data = res;

                result.status = APIStatus.success;

            }
            catch (Exception ex)
            {
                result.status = APIStatus.error;
                result.message = ex.Message;
            }

            return result;

        }

        #endregion

    }
}
