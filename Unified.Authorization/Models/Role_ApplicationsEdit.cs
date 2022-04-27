
using System.Collections.Generic;

namespace Unified.Authorization.Models
{
    public class Role_ApplicationsEdit
    {
        public ApplicationRole Role { get; set; }
        public List<HraApplications> Members { get; set; }
        public List<HraApplications> NonMembers { get; set; }
    }

    public class Role_ApplicationsModification
    {
        public string RoleId { get; set; }

        public string[] AddIds { get; set; }

        public string[] DeleteIds { get; set; }
    }
}