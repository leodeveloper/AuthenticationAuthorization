using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Unified.Authorization.Pages.ListUsersModel;

namespace Unified.Authorization.Helper
{
    public class BasePageModel : PageModel
    {
        public IActionResult OnPostSetLanguageAsync(string culture, string returnUrl)
        {
            try
            {
                Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });

                if (!string.IsNullOrEmpty(returnUrl))
                {
                    if (Convert.ToString(returnUrl).Trim() == "/")
                        return RedirectToPage("Index");
                    else
                    {
                        if (string.IsNullOrEmpty(returnUrl))
                            return RedirectPermanent(returnUrl);

                        //Below is not required as permanet redirect take cares of route value
                        else if (returnUrl.Trim().ToLower().Contains("users/list"))
                        {
                            var routeParams = new
                            {
                                UserPageNo = 1,
                                UserPageSize = 10,
                                RolePageNo = 1,
                                RolePageSize = 10,
                                UserInfoView = enumUserInfoView.Roles
                            };

                            return RedirectToPage("ListUsers", routeParams);

                            /*
                            //two case : 1 : with route
                            string[] sUrlarray = returnUrl.Split("/");
                            if (sUrlarray.Length == 3)
                            {
                                return RedirectToPage(Convert.ToString(Convert.ToString(sUrlarray[1])), new { rid = Convert.ToString(sUrlarray[2]) });
                            }
                            //2 : without route
                            else
                                return RedirectToPage(Convert.ToString(returnUrl));
                            */

                        }
                        else if (returnUrl.Trim().ToLower().Contains("roles/list"))
                        {
                            var routeParams = new
                            {
                                PageNo = 1,
                                PageSize = 10
                            };

                            return RedirectToPage("ListRoles", routeParams);

                        }
                        else if (returnUrl.Trim().ToLower().Contains("roles/edit"))
                        {
                            var routeParams = new
                            {
                                PageNo = 1,
                                PageSize = 10
                            };

                            // Go to the previous list
                            return RedirectToPage("ListRoles", routeParams);
                        }
                        else if (returnUrl.Trim().ToLower().Contains("roles/create"))
                        {
                            return RedirectToPage("CreateRole");
                        }
                        else
                            return RedirectToPage("Index");
                    }

                }

                else
                    return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                Log.Error("BasePageModel:OnPostSetLanguageAsync :: Error message => {error}", ex.Message);
                return RedirectToPage("Index");
            }

        }
    }
}

