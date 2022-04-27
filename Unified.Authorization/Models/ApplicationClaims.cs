using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unified.Authorization.Models
{

    public class ApplicationClaims
    {
        public HraApplications Application { get; set; }

        public List<ClaimType> ClaimTypes { get; set; }

    }

}
