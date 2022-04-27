using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Unified.Authorization.Models
{
    public class RoleEdit
    {     
        public ApplicationRole Role { get; set; }
        public List<ApplicationUser> Members { get; set; }
        public List<ApplicationUser> NonMembers { get; set; }
    }
 

  
}
