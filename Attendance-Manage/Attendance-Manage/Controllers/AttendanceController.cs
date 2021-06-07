using Attendance_Manage.Models;
using Attendance_Manage.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Attendance_Manage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;
        public AttendanceController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Attendance attendance)
        {
            if (attendance == null || !ModelState.IsValid)
                return BadRequest(new { message = "Invalid request body", status_code = HttpStatusCode.BadRequest });

            var attendance_id = await _attendanceService.CreateAttendance(attendance);

            attendance.attendance_id = attendance_id;
            return Ok(attendance);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var attendance = await _attendanceService.GetAttendanceAsync(id);
            if(attendance != null)
                return Ok(attendance);
            return NotFound(new { message = "No attendance found", status_code = HttpStatusCode.NotFound });
        }
    }
}
