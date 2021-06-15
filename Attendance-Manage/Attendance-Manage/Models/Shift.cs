using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        /*[DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh:mm:ss}")]*/
        public string shift_start { get; set; }
        public string shift_end { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }

    }
}
