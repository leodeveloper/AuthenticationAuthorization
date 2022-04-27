using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unified.Authorization.Models
{
    public class RolesView
    {


        #region << Fields - Properties >>

        readonly IRoleRepository roleRepo;

        public IList<ApplicationRole> Roles { get; set; }

        #endregion

        #region << Bound Properties >>

        /// <summary>
        /// I used PageNo rather than Page because it is a reserved keyword
        /// </summary>
        [BindProperty(SupportsGet = true, Name = "Page")]
        public int PageNo { get; set; }

        [BindProperty(SupportsGet = true)]
        public int RowCount { get; set; }

        public int TotalRowCount { get; set; }

        #endregion

    }
}
