using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Unified.Authorization.Models
{
    public class ApplicationRole: IdentityRole
    {             
        [DisplayName("Arabic Name")]
        public string ArName { get; set; }
                
        public bool Disabled { get; set; }

        public string Description { get; set; }


        [DisplayName("Users Count")]
        [NotMapped]
        public int UserCount { get; internal set; }

    }

}
