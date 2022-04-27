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
    public class ListUsersModel : BasePageModel
    {
        #region << Declarations >>

        public enum enumUserInfoView
        {
            Roles = 1, 
            Rights = 2,
            ContactInfo = 3
        }

        #endregion 

        #region << Fields - Properties>>

        readonly IUsersEditRepository repository;

        public Webresponse<UsersEdit> ViewModel { get; set; }

        [BindProperty(SupportsGet =true)]
        public string SearchText { get; set; }

        [BindProperty(SupportsGet = true)]
        public string UserId { get; set; } 

        [BindProperty(SupportsGet = true)]
        public int UserPageNo { get; set; }

        [BindProperty(SupportsGet = true)]
        public int UserPageSize { get; set; } 

        [BindProperty(SupportsGet = true)]
        public int RolePageNo { get; set; }

        [BindProperty(SupportsGet = true)]
        public int RolePageSize { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public enumUserInfoView? UserInfoView { get; set; } 

        #endregion

        #region << Constructors >>

        public ListUsersModel(IUsersEditRepository repository)
        {
            this.repository = repository;
        }

        #endregion

        #region << Private Methods >>

        private void SetDefaults()
        {
            UserPageNo = 1; UserPageSize = 100; RolePageNo = 1; RolePageSize = 100;
            UserInfoView = enumUserInfoView.Roles;
        }

        #endregion

        #region << Page Handlers >>

        /// <summary>
        /// For outside links 
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> OnGetOutside()
        {
            SetDefaults();
            return await OnGet();
        }

        public async Task<ActionResult> OnGet() 
        {            
            ViewModel = await repository.Get(UserId, SearchText, UserPageNo, UserPageSize, RolePageNo, RolePageSize);

            if (ViewModel.status != APIStatus.success)
            {
                ModelState.AddModelError("OnGetUser", ViewModel.message);
                return RedirectToPage("Index");
            }

            // Make sure that the selected user info view is good

            if (ViewModel.data.ContactInfo == null && UserInfoView == enumUserInfoView.ContactInfo)
                UserInfoView = enumUserInfoView.Roles;
                
            return Page();
        }


        public async Task<ActionResult> OnGetLock(bool locked)            
        {
            var response = await repository.Lock(UserId, locked);

            if (response.status != APIStatus.success)
                ModelState.AddModelError("OnGetLockUser", response.message);

            return await OnGet(); 

        }

        public async Task<ActionResult> OnPost(IList<string> GivenRoleIds)
        {
            if (ModelState.IsValid)
            {
                var response = await repository.Save(UserId, GivenRoleIds);

                if (response.status != APIStatus.success)                    
                    ModelState.AddModelError("ListUsers", response.message);                
            }

            return await OnGet(); 
        }


        public async Task<ActionResult> OnPostSearch()
        {
            SetDefaults();

            ViewModel = await repository.Get(UserId, SearchText, UserPageNo, UserPageSize, RolePageNo, RolePageSize);

            if (ViewModel.status != APIStatus.success)
            {
                ModelState.AddModelError("OnGetUser", ViewModel.message);
                return RedirectToPage("Index");
            }

            // Make sure that the selected user info view is good

            if (ViewModel.data.ContactInfo == null && UserInfoView == enumUserInfoView.ContactInfo)
                UserInfoView = enumUserInfoView.Roles;

            return Page();
        }

        #endregion

    }

}
