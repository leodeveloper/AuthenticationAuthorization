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
    /// Template
    /// </summary>
    public class EditRoleModel : BasePageModel
    {

        #region << Properties - Fields >>
                
        public RoleEdit Data { get; set; }

        readonly IRoleEditRepository repository;        

        #endregion

        public EditRoleModel(IRoleEditRepository repository)
        {
            this.repository = repository;            
        }    

        public async Task<IActionResult> OnGet(string RoleId)
        {
            var response = await repository.GetEditData(RoleId);

            if (response.status != APIStatus.success)
            {
                ModelState.AddModelError("Error", response.message);
                return RedirectToPage("ListRoles");
            }
            
            Data = response.data;
            return Page();                        
        }

        public async Task<IActionResult> OnPost(RoleModify roleModification)
        {
            if (ModelState.IsValid)
            {
                var response = await repository.Update(roleModification);

                if (response.status == APIStatus.success)
                    return RedirectToPage("ListRoles");
                    
                else
                    ModelState.AddModelError("ListRoles", response.message);
            }

            return Page();
        }

    }
}
