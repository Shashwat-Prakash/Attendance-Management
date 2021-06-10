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
        public DateTime? time_in { get; set; }        
        public DateTime? time_out { get; set; }
        [JsonPropertyName("break")]
        public IEnumerable<Break> break_ { get; set; }

        //public DateTime? current_day { get; set; }
        //public double hours { get; set; }
        //public double delay { get; set; }
        /*public string status { get; set; }
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime? last_time_in { get; set; }
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime? last_time_out { get; set; }*/

    }
}
