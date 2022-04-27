
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unified.Authorization.Models
{

    /// <summary>
    /// Helper class for editing purposes
    /// </summary>
    public class RoleClaimsEdit
    {
        public ApplicationRole Role { get; set; }

        /// <summary>
        /// Selected ApplicationId
        /// </summary>
        public Guid ApplicationId { get; set; }

        public string ApplicationName { get; set; }

        /// <summary>
        /// The Selected ModuleId, based on this field we should control how the EditRights view looks like
        /// </summary>
        public int? ModuleId { get; set; }

        public IList<HraApplications> Applications { get; set; }
        
        public IList<Guid> AssessibleApps { get; set; }

        public IList<Module> Modules { get; set; }

        public IList<ClaimType> ClaimTypes { get; set; }

        public IList<ClaimType> GivenClaims { get; internal set; }
    }


}
