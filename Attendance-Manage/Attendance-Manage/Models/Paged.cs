using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Attendance_Manage.Models
{
    public class Paged
    {
        public int offset { get; set; } = 0;
        public int limit { get; set; } = 100;
        public string sort { get; set; }
        public string order { get; set; } = "desc";
    }
}
