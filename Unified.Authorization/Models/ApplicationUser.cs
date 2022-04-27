using Microsoft.AspNetCore.Identity;
using System;

namespace Unified.Authorization.Models
{

    /// <summary>
    /// This is matching Suleman's class, only namespace is different
    /// </summary>
    public class ApplicationUser: IdentityUser
    {        
        //When the next time user login he must needs to resert the password
        public bool ispasswordreset { get; set; }
        
    }
}
