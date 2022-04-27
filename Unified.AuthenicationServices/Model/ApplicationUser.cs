using ApplicationPOCO;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using Unified.Commonlibs.Model;

namespace Unified.Model
{
    public class ApplicationUser: IdentityUser
    {

        //When the next time user login he must needs to resert the password
        public bool ispasswordreset { get; set; }

      
    }


    public class VerifyToken
    {
        public string token { get; set; }
    }

    public class VerifyTokenApplication : VerifyToken
    {
        public string ApplicationId { get; set; }
    }

    public class VerifyTokenResponseModel
    {

        public VerifyTokenResponseModel()
        {
            this.ClaimModels = new List<ClaimModel>();
            this.UserRoles = new List<UserRoles>();
            this.ContactInfo = new ContactInfo();
        }

        public IList<ClaimModel> ClaimModels { get; set; }
        public IList<UserRoles> UserRoles { get; set; }
        public ContactInfo ContactInfo { get; set; }
    }   
}
