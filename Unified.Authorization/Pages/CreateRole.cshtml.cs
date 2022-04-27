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
    public class CreateRoleModel : BasePageModel
    {

        #region << Fields - Properties >>

        [BindProperty]
        public ApplicationRole Role { get; set; }

        readonly IRoleRepository roleRepo;

        //public IList<ApplicationRole> Roles { get; set; }

        #endregion 

        public CreateRoleModel(IRoleRepository roleRepo) 
        {
            this.roleRepo = roleRepo;
        }

        public void OnGet()
        {
            Role = new ApplicationRole { Disabled = false };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {                
                //if (Role.Id == 0) {
                
                var response = await roleRepo.Add(Role);

                if (response.status == APIStatus.success)
                    return RedirectToPage("ListRoles");
                else                
                    ModelState.AddModelError("Error", response.message);
            }

            return Page();
        }

    }

}
