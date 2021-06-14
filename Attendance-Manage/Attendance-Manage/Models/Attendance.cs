using Attendance_Manage.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Attendance_Manage.Models
{
    public class Attendance
    {
        public long attendance_id { get; set; }
        public long user_id { get; set; }
        public long org_id { get; set; }
        public long shift_id { get; set; }
        public DateTime? time_in { get; set; }        
        public DateTime? time_out { get; set; }
        [JsonPropertyName("breaks")]
        public IEnumerable<Break> break_ { get; set; }
        public AttendanceStatus status { get; set; }
        public string comment { get; set; }
        public DateTime created_at { get; set; } = DateTime.UtcNow;
        public DateTime? updated_at { get; set; }        
        //public DateTime? current_day { get; set; }
        //public double hours { get; set; }
        //public double delay { get; set; }
        /*public string status { get; set; }  ------> Status can be absent or present or half-time or etc
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime? last_time_in { get; set; }
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime? last_time_out { get; set; }*/

    }
}
