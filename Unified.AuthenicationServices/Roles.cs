using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unified.AuthenicationServices
{
    /*
     {roles:{
        access:["read","create"],
    }}
     
     */
    public enum actions
    {
        read =1,
        create,
        update,
        delete
    }
    public class Roles
    {
        public List<actions> access { get; set; }
    }


}
