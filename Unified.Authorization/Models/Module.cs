using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unified.Authorization.Models
{
    public class Module
    {
        /*
        Id int identity(1,1), 
        EnName nvarchar(300) not null, 
        ArName nvarchar(300),
        ApplicationId uniqueidentifier 
        */

        public int Id { get; set; }
        public string EnName { get; set; }
        public string ArName { get; set; }
        public Guid ApplicationId { get; set; }        
    
    }

}
