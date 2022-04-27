using ApplicationPOCO;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Unified.Commonlibs.Service;
using Unified.CommonServices.Respository;
using Unified.Model;

namespace Unified.AuthenicationServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthActiveDirectoryController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        
        //https://code-maze.com/data-protection-aspnet-core/
        //https://www.c-sharpcorner.com/article/authentication-and-authorization-in-asp-net-core-web-api-with-json-web-tokens/
        private readonly IDataProtector _protector;
        private readonly ICommunicationService _iCommunicationService;
        private readonly IJwtAuthentication _jwtAuthenticationManager;
        private readonly IRedisRespository _iredisRespository;
        private const string UserId = "UserId";
        public AuthActiveDirectoryController(IRedisRespository iredisRespository, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IDataProtectionProvider provider, ICommunicationService iCommunicationService, IJwtAuthentication jwtAuthenticationManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            _configuration = configuration;
            _protector = provider.CreateProtector(_configuration["DataProtector:Key"]);
            _iCommunicationService = iCommunicationService;
            _jwtAuthenticationManager = jwtAuthenticationManager;
            _iredisRespository = iredisRespository;
        }

        [HttpGet("GetIsUserInActiveDirectory")]
        public IActionResult GetIsUserInActiveDirectory(string userName)
        {
            try
            {
                Webresponse<string> response =  CheckUserInwindowActiveDirectory(userName);
                if(response.status == APIStatus.success)
                {
                    return Ok(response);
                }
                else
                {
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new Webresponse<string> { status = APIStatus.error, message = ex.Message });
            }
        }

       

        [HttpPost("Windowslogin")]
        public async Task<IActionResult> Windowslogin([FromBody] WindowsLoginModel model)
        {
            Webresponse<string> response = CheckUserInwindowActiveDirectory(model.Username);
            var user = await userManager.FindByNameAsync(model.Username);
            if (user != null && response.status == APIStatus.success)
            {
                var IsUserLocked = await userManager.IsLockedOutAsync(user);
                if (!IsUserLocked)
                {
                    var userRoles = await userManager.GetRolesAsync(user);
                    string encryptedUserId = user.Id;//_protector.Protect(user.Id);
                    string encryptedUserName = user.UserName;//_protector.Protect(user.UserName);
                    var authClaims = new List<Claim>
                {

                    new Claim(ClaimTypes.Name.ToString(), encryptedUserName),
                    new Claim(UserId, encryptedUserId),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                    foreach (var userRole in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    }

                    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                    var token = new JwtSecurityToken(
                        issuer: _configuration["JWT:ValidIssuer"],
                        audience: _configuration["JWT:ValidAudience"],
                        expires: DateTime.Now.AddMinutes(gettokenExpire()),
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                        );
                    string serilizeToken = new JwtSecurityTokenHandler().WriteToken(token);
                    bool isTokenSaveinCache = _iredisRespository.PushValuebykey(encryptedUserId, serilizeToken).data;
                    if (isTokenSaveinCache)
                    {
                        Response.Headers.Add("X-Authorize", serilizeToken);
                        return Ok(new Webresponse<LoginResponseModel>
                        {
                            status = APIStatus.success,
                            message = APIStatus.success.ToString(),
                            data = new LoginResponseModel { Token = serilizeToken, ispasswordreset = user.ispasswordreset }
                        });
                    }
                }
            }
         
            return Unauthorized(new Webresponse<string>
            {
                status = APIStatus.error,
                message = "username or password is wrong or user is locked",
                data = null
            });
        }


        private Webresponse<string> CheckUserInwindowActiveDirectory(string userName)
        {
            try
            {
                using (var context = new PrincipalContext(ContextType.Domain, _configuration["ActiveDirectory:Url"]))
                {
                    using (var searcher = new PrincipalSearcher(new UserPrincipal(context)))
                    {
                        var isAvailable = searcher.FindAll().SingleOrDefault(z => z.SamAccountName == userName || z.SamAccountName == userName.ToLower() || z.SamAccountName == userName.ToUpper());
                        if (isAvailable != null)
                        {
                            return new Webresponse<string> { status = APIStatus.success, data = userName, message = $"{userName} exists in active directory" };
                        }
                        else
                        {
                            return new Webresponse<string> { status = APIStatus.error, message = $"{userName} not found in active directory" };
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Log.Error("AuthActiveDirectory:::Windows Authentication ::::: " + ex.Message);
                return new Webresponse<string> { status = APIStatus.error, message = $"{userName} not found in active directory, {ex.Message}" };
            }            
        }

        private long gettokenExpire()
        {
            try
            {
                long expire;
                if (Int64.TryParse(_configuration["JWT:JWTTokenExpire"], out expire))
                    return expire;
                else
                    return 1440;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return 1440;
            }
        }
    }
}
