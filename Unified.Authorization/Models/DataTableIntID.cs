using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Unified.Authorization.Models
{
    public class DataTableIntID : DataTable
    {

        public DataTableIntID()
        {
            var col = new DataColumn("Id", typeof(int));
            Columns.Add(col);
        }

        public DataTableIntID(IEnumerable<int> IDs) : this()
        {
            if (IDs != null)
                foreach (var Id in IDs)
                {
                    var row = NewRow(); row["Id"] = Id;
                    Rows.Add(row);
                }

        }
    }
}
