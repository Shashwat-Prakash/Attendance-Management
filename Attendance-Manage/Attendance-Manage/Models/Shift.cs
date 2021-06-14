using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Attendance_Manage.Models
{
    public class Shift
    {
        public long shift_id { get; set; }
        public long org_id { get; set; }
        public string name { get; set; }
        public string color { get; set; }
        public DateTime shift_start { get; set; }
        public DateTime shift_end { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }

    }
}
