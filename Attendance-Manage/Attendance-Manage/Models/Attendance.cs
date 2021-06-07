using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Attendance_Manage.Models
{
    public class Attendance
    {
        public long attendance_id { get; set; }
        public long emp_id { get; set; }
        public DateTime time_in { get; set; }
        public DateTime time_out { get; set; }
        public DateTime current_date { get; set; }
        public int hours { get; set; }
        public int delay { get; set; }
        public string status { get; set; }
        public DateTime last_time_in { get; set; }
        public DateTime last_time_out { get; set; }

    }
}
