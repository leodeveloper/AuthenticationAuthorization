using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unified.Authorization.Models
{

    /// <summary>
    /// Before edit
    /// </summary>
    public class UserEdit
    {
        public ApplicationUser User { get; set; }
        public IEnumerable<ApplicationRole> Members { get; set; }
        public IEnumerable<ApplicationRole> NonMembers { get; set; }
    }

 
    /*
    public class Users
    {
        public IEnumerable<ApplicationUser> Members { get; set; }
    }
    */

}
