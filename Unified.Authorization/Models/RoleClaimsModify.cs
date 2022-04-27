using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unified.Authorization.Models
{

    public class RoleClaimsModify
    {

        #region << Properties >>

        public string RoleId { get; set; }

        public Guid ApplicationId { get; set; }

        public int? ModuleId { get; set; }

        /// <summary>
        /// granted ClaimTypeIds
        /// </summary>
        public int[] GrantIds { get; set; }

        #endregion 

    }
    
}
