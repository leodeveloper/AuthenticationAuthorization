using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unified.Authorization.Models
{
    
    public class UserModify
    {
        public string UserId { get; set; }

        public string[] AddIds { get; set; }

        public string[] DeleteIds { get; set; }
    }

}
