using ApplicationPOCO;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Unified.AuthenicationServices.Helper
{
    //this custom authorization only for thirdparty api like TAMM integration
    [AttributeUsage(AttributeTargets.Method)]
    public class CustomAuthorization : Attribute, IAuthorizationFilter
    {


        /// <summary>  
        /// This will Authorize User  
        /// </summary>  
        /// <returns></returns>  
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {

            if (filterContext != null)
            {
                Microsoft.Extensions.Primitives.StringValues APIKeys, APIName;
                filterContext.HttpContext.Request.Headers.TryGetValue("APIName", out APIName);
                filterContext.HttpContext.Request.Headers.TryGetValue("APIKey", out APIKeys);

                var _token = APIKeys.FirstOrDefault();

                if (_token != null)
                {
                    string authToken = _token;
                    if (authToken != null)
                    {
                        if (IsValidToken(authToken, APIName))
                        {
                            filterContext.HttpContext.Response.Headers.Add("APIName", APIName);
                            filterContext.HttpContext.Response.Headers.Add("APIKey", authToken);
                            filterContext.HttpContext.Response.Headers.Add("AuthStatus", "Authorized");
                            //filterContext.HttpContext.Response.Headers.Add("storeAccessiblity", "Authorized");

                            return;
                        }
                        else
                        {
                            filterContext.HttpContext.Response.Headers.Add("APIKey", authToken);
                            filterContext.HttpContext.Response.Headers.Add("AuthStatus", "NotAuthorized");

                            filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                            filterContext.HttpContext.Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "Not Authorized";
                            filterContext.Result = new JsonResult("NotAuthorized")
                            {
                                Value = new
                                {
                                    Status = "Error",
                                    Message = "Invalid APIKey or APINAME"
                                },
                            };
                        }

                    }

                }
                else
                {
                    filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                    filterContext.HttpContext.Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "Please Provide APIKey";
                    filterContext.Result = new JsonResult("Please Provide APIKey or APINAME")
                    {
                        Value = new
                        {
                            Status = "Error",
                            Message = "Please Provide APIKey and APINAME in the header"
                        },
                    };
                }
            }
        }

        public bool IsValidToken(string apiKey, string thirdPartApiName)
        {
            //validate Token here  
            string result = Cipher.Encrypt($"Give the access to {thirdPartApiName} api");
            if (result == apiKey)
                return true;

            return false;
        }

        //for generation the api key //
        //Call this method like Cipher.Encrypt($"Give the access to TAMM api");
    }
}
