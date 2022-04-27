using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationPOCO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Unified.Authorization.Helper;
using Unified.Authorization.Models;
using Unified.Authorization.Repository;

namespace Unified.Authorization.Pages
{

    /// <summary>
    /// Apply the paging and get the users count with each role
    /// </summary>
    public class RolesModel : BasePageModel
    {

        #region << Fields - Properties >>
                
        readonly IRoleRepository roleRepo;
                
        [BindProperty]
        public WebresponsePaging<IList<ApplicationRole>> Roles { get; set; }

        #endregion

        public RolesModel(IRoleRepository roleRepo)
        {
            this.roleRepo = roleRepo;
            Roles = new WebresponsePaging<IList<ApplicationRole>> ();            
        }
        
        /*
        public void OnGet()
        {
            //Role = new ApplicationRole { Disabled = false };
            var response = roleRepo.GetAll();

            if (response.status == APIStatus.success)
                Roles = response.data;
            else
                ModelState.AddModelError("ListRoles", response.message);
        }
        */


        /// <summary>
        /// Disable a specific role
        /// </summary>
        /// <param name="RoleId"></param>
        /// <param name="Disabled"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnGetDisable(string RoleId, bool? Disabled, int pageNumber, int pageSize)
        {
            var response = await roleRepo.Disable(RoleId, Disabled.Value);

            if (response.status != APIStatus.success)
                ModelState.AddModelError("ListRoles", response.message);
            
            return await OnGet(pageNumber, pageSize);
        }

               
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageNumber">comes through routing to be used and be given the webresponse</param>
        /// <returns></returns>
        public async Task<IActionResult> OnGet(int pageNumber, int pageSize) 
        {
            if (pageNumber < 1 || pageSize < 1) { pageNumber = 1; pageSize = 100; }

            Roles = await roleRepo.GetAll(pageNumber, pageSize, null);

            if (Roles.status != APIStatus.success)
                ModelState.AddModelError("ListRoles", Roles.message);

            return Page();
        }

    }

}
