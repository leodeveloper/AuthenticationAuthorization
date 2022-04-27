using ApplicationPOCO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Unified.AuthenicationServices.Helper;
using Unified.AuthenicationServices.Model;
using Unified.Commonlibs.Model;
using Unified.Commonlibs.Service;
using Unified.CommonServices.Respository;
using Unified.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Unified.AuthenicationServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration _configuration;
        //https://code-maze.com/data-protection-aspnet-core/
        //https://www.c-sharpcorner.com/article/authentication-and-authorization-in-asp-net-core-web-api-with-json-web-tokens/
       // private readonly IDataProtector _protector;
        private readonly ICommunicationService _iCommunicationService;
        private readonly IJwtAuthentication _jwtAuthenticationManager;
        private readonly IRedisRespository _iredisRespository;        
        private const string UserId = "UserId", UserType="UserType", IsInternal= "IsInternal";

        public AuthController(IRedisRespository iredisRespository, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IDataProtectionProvider provider, ICommunicationService iCommunicationService, IJwtAuthentication jwtAuthenticationManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            _configuration = configuration;
           // _protector = provider.CreateProtector(_configuration["DataProtector:Key"]);
            _iCommunicationService = iCommunicationService;
            _jwtAuthenticationManager = jwtAuthenticationManager;
            _iredisRespository = iredisRespository;           
        }


        [HttpGet("GetAllUsers")]
        public IActionResult GetUsersAsync()
        {
            return Ok(userManager.Users.ToList());
        }



        [HttpGet("GetUserByEmail")]
        public async Task<IActionResult> GetUserByEmail(string useremail)
        {
            try
            {
                var userExists = await userManager.FindByEmailAsync(useremail);
                if (userExists != null)
                {
                    return Ok(new Webresponse<string> { status = APIStatus.success, data = userExists.UserName });
                }
                else
                    return BadRequest(new Webresponse<string> { status = APIStatus.error, message = "User or email address already exists!" });
            }
            catch(Exception ex)
            {
                return BadRequest(new Webresponse<string> { status = APIStatus.error, message = ex.Message });
            }           
        }

        [HttpGet("GetUserByUserName")]
        public async Task<IActionResult> GetUserByUserName(string userName)
        {
            try
            {
                var userExists = await userManager.FindByNameAsync(userName);
                if (userExists != null)
                {
                    return Ok(new Webresponse<string> { status = APIStatus.success, data = userExists.Id });
                }
                else
                    return BadRequest(new Webresponse<string> { status = APIStatus.error, message = "UserName does not exists" });
            }
            catch (Exception ex)
            {
                return BadRequest(new Webresponse<string> { status = APIStatus.error, message = ex.Message });
            }
        }


        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(new Webresponse<string> { status = APIStatus.error , message  = "model validtion failed" });
                }
                var userExists = await userManager.FindByNameAsync(model.Username);
                var userEmailExists = await userManager.FindByEmailAsync(model.Email);
                if (userExists != null || userEmailExists != null)
                    return BadRequest(new Webresponse<string> { status = APIStatus.error, message = "User or email address already exists!" });

                ApplicationUser user = new ApplicationUser()
                {
                    Email = model.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = model.Username,
                    ispasswordreset = false
                };
                var result = await userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                    return BadRequest(new Webresponse<string> { status = APIStatus.error, message = "User creation failed! Please check user details and try again." });

                return Ok(new Webresponse<string> { status = APIStatus.success, message = "User created successfully!" });
            }
            catch (Exception ex) 
            {
                return BadRequest(new Webresponse<string> { status = APIStatus.error, message = "User creation failed! Please check user details and try again." + ex.Message });
            }
           
        }

        [HttpPost]
        [Route("registerWindowsUser")]
        public async Task<IActionResult> RegisterWindowsUser([FromBody] RegisterModel model)
        {
            try
            {
                model.Password = GenerateRandomPassword();
               return await Register(model);
            }
            catch (Exception ex)
            {
                return BadRequest(new Webresponse<string> { status = APIStatus.error, message = "User creation failed! Please check user details and try again." + ex.Message });
            }

        }

        /// <summary>
        /// For Gamal API the user already login with another system
        /// </summary>
        /// <param name="emiratesId"></param>
        /// <returns></returns>
        [HttpPost("ThirdPartyLogin")]
        public async Task<IActionResult> ThirdPartyLogin(string emiratesId)
        {
            EnumUserType enumUserType;
            string strError = string.Empty;

            Webresponse<ContactInfo> WebResponse = new Webresponse<ContactInfo>();
            using (HttpClient _httpClient = new HttpClient())
            {

                WebResponse = await _httpClient.GetFromJsonAsync<Webresponse<ContactInfo>>(_configuration["ContactAPI:Url"] + "/GetContactByEmiratesId?emiratesId=" + emiratesId);
                if (WebResponse.status == APIStatus.success)
                {
                    enumUserType = (EnumUserType)Enum.ToObject(typeof(EnumUserType), WebResponse.data?.UserType);
                    if (!enumUserType.Equals(EnumUserType.ThridPartyUser))
                    {
                        return Unauthorized(new Webresponse<string>
                        {
                            status = APIStatus.error,
                            message = "Emirates does not allow for thirdparty login ",
                            data = null
                        });
                    }
                }
                else
                {
                    return Unauthorized(new Webresponse<string>
                    {
                        status = APIStatus.error,
                        message = "Emirates Id not found in the contacts",
                        data = null
                    });
                }
            }
            var user = await userManager.FindByEmailAsync(WebResponse.data.Emailid);
            if (user != null)
            {
                var IsUserLocked = await userManager.IsLockedOutAsync(user);
                if (!IsUserLocked)
                {
                    var userRoles = await userManager.GetRolesAsync(user);
                    //  string encryptedUserId = _protector.Protect(user.Id);
                    //  string encryptedUserName = _protector.Protect(user.UserName);
                    var authClaims = new List<Claim>
                {

                    new Claim(ClaimTypes.Name.ToString(), user.UserName),
                    new Claim(UserId, user.Id),
                    new Claim(UserType, WebResponse.data?.UserType.ToString()),
                    new Claim(IsInternal, WebResponse.data?.IsInternal.ToString()),
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
                    bool isTokenSaveinCache = _iredisRespository.PushValuebykey(user.Id, serilizeToken).data;
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
                    else
                    {
                        strError += "Cahing failed";
                        Log.Error("Cahing failed");
                    }
                }
                else
                {
                    strError += $"error: isuserlocked--  {IsUserLocked} -- Username ";
                }
            }
            else
            {
                strError += $"Emirates Id {emiratesId} found in the Contacts but User not found";
                Log.Error(strError);
            }

            return Unauthorized(new Webresponse<string>
            {
                status = APIStatus.error,
                message = "username or password is wrong or user is locked ---- " + strError,
                data = null
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            string strError = string.Empty;
            var user = await userManager.FindByNameAsync(model.Username);
            if(user != null)
            {
                var IsUserLocked = await userManager.IsLockedOutAsync(user);
                var IsUserValidUserandPassword = await userManager.CheckPasswordAsync(user, model.Password);
                if (!IsUserLocked && IsUserValidUserandPassword)
                {
                    var userRoles = await userManager.GetRolesAsync(user);
                  //  string encryptedUserId = _protector.Protect(user.Id);
                  //  string encryptedUserName = _protector.Protect(user.UserName);
                    var authClaims = new List<Claim>
                {

                    new Claim(ClaimTypes.Name.ToString(), user.UserName),
                    new Claim(UserId, user.Id),
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
                    bool isTokenSaveinCache = _iredisRespository.PushValuebykey(user.Id, serilizeToken).data;
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
                    else
                    {
                        strError += "Cahing failed";
                        Log.Error("Cahing failed");
                    }
                }
                else
                {
                    strError += $"error: isuserlocked--  {IsUserLocked} -- Username or password wrong {IsUserValidUserandPassword}";
                }
            }
            else
            {
                strError += "User not found";
                Log.Error("User not found");
            }
           
            return Unauthorized(new Webresponse<string>
            {
                status = APIStatus.error,
                message = "username or password is wrong or user is locked ---- " + strError,
                data = null
            });
        }


        

        [HttpPost("ForgotResetPassword")]
        public async Task<IActionResult> ForgotResetPassword(ForgotPasswordModel forgotPasswordModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var user = await userManager.FindByEmailAsync(forgotPasswordModel.Email);            
            if (user == null)
                return BadRequest();
            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            var NewPassword = GenerateRandomPassword();
            //When the next time user login he must needs to resert the password
            user.ispasswordreset = true;
            var resetPassResult = await userManager.ResetPasswordAsync(user, token, NewPassword);
            await _iCommunicationService.PostRequest(_configuration["EmailAPI:Url"], new Commonlibs.Model.EmailModel { Body = $"User Name {user.UserName} <br /> New password {NewPassword}", Subject = "Reset password ", ToEmail = forgotPasswordModel.Email, ToEmailName = forgotPasswordModel.Email });
            return Ok(new Webresponse<string>
            {
                status = APIStatus.success,
                message = "Password successfully changed, please check your email",
                data = null
            });
        }

        [HttpPost("ForgotPassword")]
       // [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel forgotPasswordModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var user = await userManager.FindByEmailAsync(forgotPasswordModel.Email);
            if (user == null)
                return BadRequest();
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            await _iCommunicationService.PostRequest(_configuration["EmailAPI:Url"], new Commonlibs.Model.EmailModel { Body=$"Reset password {token}", Subject= "Reset password Token ", ToEmail= forgotPasswordModel.Email, ToEmailName= forgotPasswordModel.Email });
            return Ok(new Webresponse<string>
            {
                status = APIStatus.success,
                message = APIStatus.success.ToString(),
                data = token
            });
        }

       
        [HttpGet("GetContactByEmail")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetContactByEmail()
        {
            try
            {
                using (HttpClient _httpClient = new HttpClient())
                {
                    var user = await userManager.FindByNameAsync(User.Identity.Name);
                    Webresponse<ContactInfo> WebResponse = await _httpClient.GetFromJsonAsync<Webresponse<ContactInfo>>(_configuration["ContactAPI:Url"] + "/GetContactbyEmail?contactEmail="+ user.Email);
                    if (WebResponse.status == APIStatus.success)
                    {
                        return Ok(WebResponse);
                    }
                    else
                    {
                        return BadRequest(WebResponse);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

      

        [HttpPost("GetContactByListUserIds")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetContactByListUserIds(string[] userIds)
        {
            try
            {
                var users = userManager.Users.Where(e => userIds.Contains(e.Id));
                using (HttpClient _httpClient = new HttpClient())
                {   
                    var response = await _httpClient.PostAsJsonAsync<string[]>(_configuration["ContactAPI:Url"] + "/GetContactbyListEmails" , users.Select(z=>z.Email).ToArray());
                    if(response.IsSuccessStatusCode)
                    {
                        return Ok(await response.Content.ReadFromJsonAsync<Webresponse<IList<ContactInfo>>>());
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("ResetPassword")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel resetPasswordModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var user = await userManager.FindByEmailAsync(resetPasswordModel.Email);            
            if (user == null)
                return BadRequest();
            if (user.UserName != User.Identity.Name)
                return BadRequest();
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            user.ispasswordreset = false;
            var resetPassResult = await userManager.ResetPasswordAsync(user, token, resetPasswordModel.Password);
            if (!resetPassResult.Succeeded)
            {
                foreach (var error in resetPassResult.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }
            return Ok(new WebresponseNoData
            {
                status = APIStatus.success,
                message = APIStatus.success.ToString()
            });
        }

        [HttpPost("ResetPasswordForContact")]
        [CustomAuthorization]
        public async Task<IActionResult> ResetPasswordForContact(ResetPasswordModel resetPasswordModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var user = await userManager.FindByEmailAsync(resetPasswordModel.Email);
            if (user == null)
                return BadRequest();
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            user.ispasswordreset = false;
            var resetPassResult = await userManager.ResetPasswordAsync(user, token, resetPasswordModel.Password);
            if (!resetPassResult.Succeeded)
            {
                foreach (var error in resetPassResult.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }
            return Ok(new WebresponseNoData
            {
                status = APIStatus.success,
                message = APIStatus.success.ToString()
            });
        }

        [HttpGet("Logout")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Logout()
        {
            IList<ClaimModel> claims = getClaimsFromToken();
            ClaimModel claimUser = claims.FirstOrDefault(z => z.ClaimType == UserId);
            string userId = claimUser.ClaimValue;
            Webresponse<bool> isRemove = _iredisRespository.RemoveValuebykey(userId);
            if(isRemove.data)
                return Ok();

            return BadRequest();
        }

        [HttpGet("verify")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> VerifyToken()
        {
            try
            {
                IList<ClaimModel> claims =  getClaimsFromToken();
                ClaimModel claimUser = claims.FirstOrDefault(z => z.ClaimType == UserId);
                string UserIdActual = claimUser.ClaimValue;
                claimUser.ClaimValue = UserIdActual;
                var userExists = await userManager.FindByIdAsync(UserIdActual);
                if (userExists == null)
                {
                    return Unauthorized();
                }
                return Ok(new Webresponse<IList<ClaimModel>> { data = claims, message = "", status = APIStatus.success });
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }            
        }


        [HttpPost("verify")]
        public async Task<IActionResult> VerifyToken(VerifyToken verifytoken)
        {
            try
            {
                using (var _httpClient = new HttpClient())
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", verifytoken.token);
                    var VerifyResponse = await _httpClient.GetAsync(_configuration["AuthenticationAPI:Url"] + "/Auth/Verify");
                    //Get Claims
                    if (VerifyResponse.IsSuccessStatusCode)
                    {
                        Webresponse<IList<ClaimModel>> webresponse = JsonConvert.DeserializeObject<Webresponse<IList<ClaimModel>>>(await VerifyResponse.Content.ReadAsStringAsync());
                        return Ok(webresponse);
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }            
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpPost("verifyToken")]
        public async Task<IActionResult> VerifyToken(VerifyTokenApplication verifytoken)
        {
            try
            {
                Webresponse<VerifyTokenResponseModel> response = new Webresponse<VerifyTokenResponseModel> { data = new VerifyTokenResponseModel() };
                using (var _httpClient = new HttpClient())
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", verifytoken.token);
                    var VerifyResponse = await _httpClient.GetAsync(_configuration["AuthenticationAPI:Url"] + "/Auth/Verify");
                    //Get Claims
                    if (VerifyResponse.IsSuccessStatusCode)
                    {
                        Webresponse<IList<ClaimModel>> webresponse = JsonConvert.DeserializeObject<Webresponse<IList<ClaimModel>>>(await VerifyResponse.Content.ReadAsStringAsync());
                        if (webresponse.status == APIStatus.success)
                        {
                            response.data.ClaimModels = webresponse.data;
                            var userId = webresponse.data.FirstOrDefault(z => z.ClaimType == UserId);
                          
                            IList<Task> tasks = new List<Task>();

                            tasks.Add(getRoles(response, _httpClient, userId.ClaimValue, verifytoken));
                            tasks.Add(getContacts(response, _httpClient, verifytoken.token));

                            Task.WaitAll(tasks.ToArray());                   
                            response.status = APIStatus.success;
                            return Ok(response);
                        }
                        else
                        {
                            return BadRequest(new Webresponse<string> { status = APIStatus.error, message = "Verify token failed" });
                        }               
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }


        #region private
        private async Task getRoles(Webresponse<VerifyTokenResponseModel> response, HttpClient _httpClient, string userId, VerifyTokenApplication verifytoken)
        {
            try
            {
                Webresponse<IList<UserRoles>> roleResponse = await _httpClient.GetFromJsonAsync<Webresponse<IList<UserRoles>>>(_configuration["AuthorizationAPI:Url"] + $"/Services/GetUserRolesAndClaims?UserId={userId}&ApplicationId={verifytoken.ApplicationId}");
                if (roleResponse.status == APIStatus.success)
                {
                    response.data.UserRoles = roleResponse.data;
                }
            }
            catch (Exception ex) { response.message += "--- Unable to get role" + ex.Message; }
           
        }

        private async Task getContacts(Webresponse<VerifyTokenResponseModel> response, HttpClient _httpClient, string verifyToken)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", verifyToken);
                Webresponse<ContactInfo> ContactResponse = await _httpClient.GetFromJsonAsync<Webresponse<ContactInfo>>(_configuration["AuthenticationAPI:Url"] + "/Auth/GetContactbyEmail");
                if (ContactResponse.status == APIStatus.success)
                {
                    response.data.ContactInfo = ContactResponse.data;
                }
            }
            catch (Exception ex) {
                response.data.ContactInfo = null;
                response.message += "--- Unable to get contact Info " + ex.Message; 
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
        private IList<ClaimModel> getClaimsFromToken()
        {
            IList<ClaimModel>  claims = new List<ClaimModel>();
            foreach (var claim in User.Claims)
            {
                claims.Add(new ClaimModel { ClaimValue = claim.Value, ClaimType = claim.Type });
            }
            return claims;
        }
        private static string GenerateRandomPassword(PasswordOptions opts = null)
        {
            if (opts == null) opts = new PasswordOptions()
            {
                RequiredLength = 8,
                RequiredUniqueChars = 4,
                RequireDigit = true,
                RequireLowercase = true,
                RequireNonAlphanumeric = true,
                RequireUppercase = true
            };

            string[] randomChars = new[] {"ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
                "abcdefghijkmnopqrstuvwxyz",    // lowercase
                "0123456789",                   // digits
                "@#"                     // non-alphanumeric
            };
            Random rand = new Random(Environment.TickCount);
            List<char> chars = new List<char>();

            if (opts.RequireUppercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[0][rand.Next(0, randomChars[0].Length)]);

            if (opts.RequireLowercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[1][rand.Next(0, randomChars[1].Length)]);

            if (opts.RequireDigit)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[2][rand.Next(0, randomChars[2].Length)]);

            if (opts.RequireNonAlphanumeric)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[3][rand.Next(0, randomChars[3].Length)]);

            for (int i = chars.Count; i < opts.RequiredLength
                || chars.Distinct().Count() < opts.RequiredUniqueChars; i++)
            {
                string rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count),
                    rcs[rand.Next(0, rcs.Length)]);
            }

            string finalPassword = new string(chars.ToArray());
            string charsCaps = randomChars[0].ToString();
            Random randomCharsCaps = new Random();
            int numFirst = randomCharsCaps.Next(0, charsCaps.Length - 1);
            int numLast = randomCharsCaps.Next(0, charsCaps.Length - 1);
            return finalPassword = charsCaps[numFirst] + finalPassword + charsCaps[numLast];
        }
        #endregion
    }

    
}
