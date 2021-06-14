using Attendance_Manage.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Attendance_Manage.Models
{
    public class Break
    {
        public long break_id { get; set; }
        public long attendance_id { get; set; }
        public long user_id { get; set; }
        public long org_id { get; set; }
        public DateTime? break_time_in { get; set; }
        public DateTime? break_time_out { get; set; }
        public BreakType type { get; set; }
        public string comment { get; set; }
        public DateTime created_at { get; set; }
        public DateTime? updated_at { get; set; }

        //public DateTime? current_day { get; set; }
        //public double break_hours { get; set; }
        /*public string status { get; set; }
        public DateTime? last_break_time_in { get; set; }
        
        public DateTime? last_break_time_out { get; set; }*/
    }
}
