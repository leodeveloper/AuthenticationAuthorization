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
    public class EditRoleClaimsModel : BasePageModel
    {

        // Define the repository         
        readonly IRoleClaimsEditRepository repository;        

        // Define the model object
        public RoleClaimsEdit Data { get; set; }

        [BindProperty(SupportsGet =true)]
        public bool ViewOnly { get; set; }


        public EditRoleClaimsModel(IRoleClaimsEditRepository repository) 
        {
            this.repository = repository;
        }

        public async Task<ActionResult> OnGet(string RoleId, Guid ApplicationId, int? ModuleId)
        {            
            var response = await repository.GetEditData(RoleId, ApplicationId, ModuleId);

            if (response.status != APIStatus.success)
            {            
                ModelState.AddModelError("GetRoleClaims", response.message);
                return RedirectToPage("ListRoles");
            }

            Data = response.data;

            return Page();
        }
                
        public async Task<ActionResult> OnPost(RoleClaimsModify Data)
        {            
            var response = await repository.SaveClaims(Data);

            if (response.status == APIStatus.success)
            {
                return RedirectToPage("EditRoleClaims", new
                {
                    Data.RoleId,
                    Data.ApplicationId,
                    Data.ModuleId
                });
            }
            else
            {
                ModelState.AddModelError("PostRoleClaims", response.message);
                return RedirectToPage("ListRoles");
            }
                
        }
    }
}
