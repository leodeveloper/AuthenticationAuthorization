using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ApplicationPOCO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Unified.AuthenicationServices.Controllers
{
   // [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PingController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<PingController> _logger;
        private readonly IConfiguration _configuration;

        public PingController(ILogger<PingController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

       
        [HttpGet("Get")]
        public IEnumerable<WeatherForecast> Get()
        {
            string key = Cipher.Encrypt("Give the access to CONTACT api");
            var userclaims = GetPrincipal();
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)],
                //token = userclaims.Claims. // (userclaims != null && userclaims.Claims != null) ? (userclaims.Claims.First(x => x.Type == "UserType").Value) : ""
            })
            .ToArray();
        }

        ClaimsPrincipal GetPrincipal()
        {
            try
            {
                string authHeader = HttpContext.Request.Headers["Authorization"];
                if (!string.IsNullOrEmpty(authHeader))
                {
                    var authHeaderValue = AuthenticationHeaderValue.Parse(authHeader);//reads 
                    if (authHeaderValue.Scheme.Equals("Bearer", StringComparison.OrdinalIgnoreCase))
                    {
                        string _token = Convert.ToString(authHeader.Split(" ")[1]);
                        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                        JwtSecurityToken jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(_token);
                        if (jwtToken == null) return null;

                        TokenValidationParameters parameters = new TokenValidationParameters()
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Secret"])),
                            ValidateIssuer = false,
                            ValidateAudience = false,
                            RequireExpirationTime = true,

                        };
                        SecurityToken securityToken;
                        ClaimsPrincipal principal = tokenHandler.ValidateToken(_token, parameters, out securityToken);
                        return principal;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }
    }
}
