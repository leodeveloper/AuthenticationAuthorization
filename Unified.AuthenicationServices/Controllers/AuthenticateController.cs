using ApplicationPOCO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Refit;
using System;
using System.Threading.Tasks;

namespace Unified.AuthenicationServices.Controllers
{
    [Headers("Authorization: Bearer", "Content-type: application/json")]

    public interface icommonAPI
    {
        [Post("/SaveTocache")]
        Task<Webresponse<bool>> SaveTocache([Body] CacheInput _input);
    }

    [ApiController]
    [Route("[controller]")]
    public class AuthenticateController : ControllerBase
    {
        private readonly IJwtAuthentication _jwtAuthenticationManager;

        public AuthenticateController(IJwtAuthentication jwtAuthenticationManager)
        {
            _jwtAuthenticationManager = jwtAuthenticationManager;

        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Credentials cred)
        {
            Webresponse<string> _result = null;
            try
            {
                _result = _jwtAuthenticationManager.Authenticate(cred);
                if (_result == null || _result.status != APIStatus.success) return Unauthorized();
                else
                {
                    string authHeader = $"Bearer {_result.data}";
                    if (!string.IsNullOrEmpty(authHeader))
                    {
                        var refitSettings = new RefitSettings
                        {
                            AuthorizationHeaderValueGetter = () =>
                            {
                                return Task.FromResult(authHeader.Split()[1]);
                            },
                        };
                        var api = RestService.For<icommonAPI>("http://localhost:61840/common/", refitSettings);

                        var someresult = await api.SaveTocache(new CacheInput { key = cachekey.uservalidlogin.ToString(), value = _result.data });


                    }

                }
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
            return Ok(_result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> logout()
        {
            Webresponse<string> _result = null;
            try
            {
                string authHeader = HttpContext.Request.Headers["Authorization"];
                if (!string.IsNullOrEmpty(authHeader))
                {
                    var refitSettings = new RefitSettings
                    {
                        AuthorizationHeaderValueGetter = async () =>
                        {
                            return await Task.FromResult(authHeader.Split()[1]);
                        },
                    };
                    var api = RestService.For<icommonAPI>("http://localhost:61840/common/", refitSettings);
                    var someresult = await api.SaveTocache(new CacheInput { key = cachekey.userinvalidlogin.ToString(), value = authHeader });


                }
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
            return Ok(_result);
        }



    }

}


