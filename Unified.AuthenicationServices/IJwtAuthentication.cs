using ApplicationPOCO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unified.AuthenicationServices
{
    public interface IJwtAuthentication
    {
        Webresponse<string> Authenticate(Credentials cred);
    }
}
