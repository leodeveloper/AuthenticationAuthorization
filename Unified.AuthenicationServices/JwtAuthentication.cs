using ApplicationPOCO;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

namespace Unified.AuthenicationServices
{
    public class JwtAuthentication : IJwtAuthentication
    {
        private readonly IDictionary<string, string> _users = new Dictionary<string, string> {
            {
                "test1","password1"
            }
        };
        private readonly string _key;
        public JwtAuthentication(string key)
        {
            _key = key;
        }
        public Webresponse<string> Authenticate(Credentials cred)
        {
            Webresponse<string> _result = new Webresponse<string>
            {
                status = APIStatus.processing,
                message = APIStatus.processing.ToString(),
                data = string.Empty
            };

            try
            {
                if (!_users.Any(user => user.Key == cred.username && user.Value == cred.password)) return null;
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenkey = Encoding.ASCII.GetBytes(_key);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.Name, cred.username),
                    new Claim("UserType", "myfuture")

                    }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenkey), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                _result.data = tokenHandler.WriteToken(token);
                _result.status = APIStatus.success;
                _result.message = APIStatus.success.ToString();

               
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _result;
        }
    }
}
