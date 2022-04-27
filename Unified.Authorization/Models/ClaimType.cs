using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Unified.Authorization.Models
{
    public class ClaimType
    {

        #region << Properties >>

        /*
        Id int identity,
	    EnTitle Nvarchar(200) not null,
	    ArTitle Nvarchar(200) not null,
	    ApplicationId uniqueidentifier not null,
        ModuleId int, 
        */

        [ReadOnly(true)]
        [Key]
        public int Id { get; set; }

        [Required]
        public string EnTitle { get; set; }

        public string ArTitle { get; set; }

        [Required]
        public Guid ApplicationId { get; set; }

        public int? ModuleId { get; set; }

        #endregion
                

    }
}
