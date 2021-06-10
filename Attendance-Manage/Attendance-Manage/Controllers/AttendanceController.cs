using Attendance_Manage.Helpers;
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
        private readonly IBreakService _breakService;
        long org_id = 3;
        public AttendanceController(IAttendanceService attendanceService, IBreakService breakService)
        {
            _attendanceService = attendanceService;
            _breakService = breakService;
        }

        /// <summary>
        /// Create an attenedance
        /// </summary>
        /// <param name="attendance"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Attendance attendance)
        {
            if (attendance == null || !ModelState.IsValid)
                return BadRequest(Logger.Error("Invalid request body"));

            //attendance.current_day = DateTime.UtcNow;            
            var attendance_id = await _attendanceService.CreateAttendance(attendance);

            attendance.attendance_id = attendance_id;

            if(attendance.attendance_id > 0 && attendance.break_ != null)
            {
                await _breakService.CreateBreak(attendance.break_.FirstOrDefault());
            }

            return Ok(attendance);
        }

        /// <summary>
        /// Get attendance by Id
        /// </summary>
        /// <param name="attendance_id"></param>
        /// <returns></returns>
        [HttpGet("{attendance_id}")]
        public async Task<IActionResult> Get(long attendance_id)
        {
            var attendance = await _attendanceService.GetAttendanceAsync(attendance_id, org_id);
            if(attendance != null)
            {
                var _break = await _breakService.GetBreakAsyncByAttendanceId(attendance.attendance_id, org_id);
                if (_break != null && _break.Any())
                {
                    attendance.break_ = _break;                    
                }
                return Ok(attendance);
            }
            return NotFound(Logger.Error("No attendance found", HttpStatusCode.NotFound));
        }

        /// <summary>
        /// Update attendacne by Id
        /// </summary>
        /// <param name="attendance"></param>
        /// <param name="attendance_id"></param>
        /// <returns></returns>
        [HttpPut("{attendance_id}")]
        public async Task<IActionResult> Update([FromBody] Attendance attendance, long attendance_id)
        {
            if (attendance == null || !ModelState.IsValid)
                return BadRequest(Logger.Error("Invalid request body"));

            /*var _break = await _breakService.GetBreakAsyncByAttendanceId(id);*/
            var attendanceResult = await _attendanceService.GetAttendanceAsync(attendance_id, org_id);

            attendance.attendance_id = attendance_id;
            attendance.org_id = org_id;

            if(attendance.time_out != null)
            {
                attendance.time_in = attendanceResult.time_in;                                
            }

            else
            {
                attendance.time_out = attendanceResult.time_out;                
            }            

            /*TimeSpan diff = (TimeSpan)(attendance.time_out - attendanceResult.time_in);
            attendance.hours = attendanceResult.hours + (int)diff.TotalHours;*/

            var isUpdated = await _attendanceService.UpdateAttendanceAsync(attendance);
            if (isUpdated)
                return Ok(Logger.Success("Attendance updated successfully"));

            return StatusCode((int)HttpStatusCode.InternalServerError, Logger.ServerError("Internal server error, pleae try again!"));
        }
    }
}
