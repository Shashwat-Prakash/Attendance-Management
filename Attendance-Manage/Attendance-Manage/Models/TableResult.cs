using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Attendance_Manage.Models
{
    public class TableResult<T>
    {
        public long? total { get; set; }
        public int? limit { get; set; }
        public int? offset { get; set; }
        public int? returned { get; set; }
        public IEnumerable<T> result { get; set; }
    }
}
