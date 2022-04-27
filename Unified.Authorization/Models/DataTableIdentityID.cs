using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Unified.Authorization.Models
{
    public class DataTableStrID: DataTable
    {

        public DataTableStrID()
        {
            var col = new DataColumn("Id", typeof(string));
            col.MaxLength = 425;

            Columns.Add(col);
        }

        public DataTableStrID(IEnumerable<string> IDs) : this()
        {
            if(IDs != null)
                foreach (var Id in IDs)
                {
                    var row = NewRow(); row["Id"] = Id;
                    Rows.Add(row);
                }

        }
    }

}
