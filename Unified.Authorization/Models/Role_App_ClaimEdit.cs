using System;
using System.Collections.Generic;
using Unified.Authorization.Models;

namespace Unified.Authorization.Models
{
  
    /// <summary>
    /// Helper Class to edit the claims of a specific (role, application)
    /// </summary>
    public class Role_App_ClaimsEdit
    {

        #region << Properties >>

        public HraApplications App { get; set; }

        public ApplicationRole Role { get; set; }

        /// <summary>
        /// All ClaimTypes
        /// </summary>
        public List<ClaimType> ClaimTypes { get; set; }

        /// <summary>
        /// Granted claims
        /// </summary>
        public List<ClaimType> GrantedClaimTypes { get; set; }

        /// <summary>
        /// Only the modules of the current application
        /// </summary>
        public List<Module> Modules { get; set; }

        #endregion 

    }

    /// <summary>
    /// For receiving the modified data
    /// </summary>
    public class Role_App_ClaimsModify
    {

        #region << Properties >>

        public Guid ApplicationId { get; set; }

        public int? ModuleId { get; set; }

        public string RoleId { get; set; }

        public int[] GrantIds { get; set; }

        #endregion 

    }

}