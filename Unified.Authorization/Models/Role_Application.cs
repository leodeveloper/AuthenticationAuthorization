using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unified.Authorization.Models
{
    
    /// <summary>
    /// Helper Class
    /// </summary>
    public class Role_Application
    {

        #region << Properties >>

        public Guid Id { get; set; }

        public Guid ApplicationId { get; set; }

        public string RoleId { get; set; }

        public ApplicationRole Role { get; set; }

        public HraApplications Application { get; set; }

        #endregion 

    }

}
