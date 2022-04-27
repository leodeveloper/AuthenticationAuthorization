using System;

namespace Unified.Authorization.Models
{
    
    public class HraApplications
    {
        // [ApplicationId], [ApplicationName], [ApplicationUrl], [ApplicationIcon], [Description]

        public Guid ApplicationId { get; set; }
        
        public string ApplicationName { get; set; }

        public string ApplicationUrl { get; set; }
                        
        public string ApplicationIcon { get; set; }

        public string Description { get; set; }

    }

}