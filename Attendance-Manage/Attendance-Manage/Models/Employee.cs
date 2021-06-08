using Attendance_Manage.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Attendance_Manage.Models
{
    public class Employee
    {
        public long emp_id { get; set; }
        public string name { get; set; }
        public string contact { get; set; }
        public string email_id { get; set; }
        public string dept_name { get; set; }
        public string designation_name { get; set; }
        public double salary { get; set; }
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime joining_date { get; set; }
        public WorkType type { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }

    }
}
