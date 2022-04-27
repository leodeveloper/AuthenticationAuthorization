using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Unified.Authorization.Models
{

    public class RoleModify : ApplicationRole
    {
        //public string RoleId { get; set; }
        //[Required]
        //public string RoleName { get; set; }

        public string[] AddIds { get; set; }

        public string[] DeleteIds { get; set; }
    }

}
