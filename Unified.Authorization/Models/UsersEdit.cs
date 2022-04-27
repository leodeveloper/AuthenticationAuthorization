using ApplicationPOCO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unified.Authorization.Models
{
    public class UsersEdit
    {
        public UsersEdit()
        {
            Users = new WebresponsePaging<IList<ApplicationUser>>();
            Roles = new WebresponsePaging<IList<ApplicationRole>>();
            GivenRoleIds = new List<string>();
        }

        public WebresponsePaging<IList<ApplicationUser>> Users { get; set; }
        public WebresponsePaging<IList<ApplicationRole>> Roles { get; set; }



        #region << Selected User >>

        /// <summary>
        /// If the user is not selected, this should be null
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// if the user is not selected, this should be null
        /// </summary>
        public ContactInfo ContactInfo { get; set; }

        /// <summary>
        /// if the user is not selected, this should be empty
        /// This contains the roles as names (not IDs)
        /// </summary>
        public IList<string> GivenRoleIds { get; set; }

        public Webresponse<IList<HraApplications>> GrantedApplications { get; set; }
        public Webresponse<IList<Module>> GrantedModules { get; set; }
        public Webresponse<IList<ClaimType>> GrantedClaims { get; set; }

        #endregion

        #region << Searching >> 

        public string SearchText { get; set; }

        #endregion

    }
}
