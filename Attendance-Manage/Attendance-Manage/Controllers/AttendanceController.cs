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
            var attendance_id = await _attendanceService.CreateAttendanceAsync(attendance);

            attendance.attendance_id = attendance_id;

            if(attendance.attendance_id > 0 && attendance.break_ != null)
            {
                attendance.break_.FirstOrDefault().attendance_id = attendance.attendance_id;
                attendance.break_.FirstOrDefault().break_id = await _breakService.CreateBreakAsync(attendance.break_.FirstOrDefault());
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
            var attendance = await _attendanceService.GetAttendanceByIdAsync(attendance_id, org_id);
            if(attendance != null)
            {
                var _break = await _breakService.GetBreakByAttendanceIdAsync(attendance.attendance_id, org_id);
                if (_break != null && _break.Any())
                {
                    attendance.break_ = _break;                    
                }
                return Ok(attendance);
            }
            return NotFound(Logger.Error("No attendance found", HttpStatusCode.NotFound));
        }

        /// <summary>
        /// Get all attendance
        /// </summary>
        /// <param name="paged"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] Paged paged, [FromQuery] IDictionary<string, string> filters)
        {
            paged.sort ??= "attendance_id";

            var attendance = await _attendanceService.GetAttendanceByOrgIdAsync(org_id, paged, filters);
            if(attendance!=null && attendance.Any())
            {
                var table = new TableResult<dynamic>
                {
                    limit = paged.limit,
                    offset = paged.offset,
                    returned = attendance.Count(),
                    total = await _attendanceService.GetAttendanceCountAsync(org_id, filters),
                    result = attendance
                };
                return Ok(table);
            }
            return NotFound(Logger.Error($"No attendance found", HttpStatusCode.NotFound));
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
          
            var attendanceResult = await _attendanceService.GetAttendanceByIdAsync(attendance_id, org_id);

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

            var isUpdated = await _attendanceService.UpdateAttendanceByIdAsync(attendance);
            if (isUpdated)
                return Ok(Logger.Success("Attendance updated successfully"));

            return StatusCode((int)HttpStatusCode.InternalServerError, Logger.ServerError("Internal server error, pleae try again!"));
        }

        /// <summary>
        /// Delete attendance by Id
        /// </summary>
        /// <param name="attendance_id"></param>
        /// <returns></returns>
        [HttpDelete("{attendance_id}")]
        public async Task<IActionResult> Delete(long attendance_id)
        {
            if (attendance_id < 1)
            {
                return BadRequest(Logger.Error("Invalid attendance id", HttpStatusCode.BadRequest));
            }

            var idsToDelete = new long[] { attendance_id };
            var rowAffected = await _attendanceService.DeleteAttendanceAsync(idsToDelete, org_id);
            if (rowAffected > 0)
            {
                return Ok(Logger.Success($"Attendance with id: {attendance_id} deleted successfully"));
            }

            return NotFound(Logger.Error($"Cannot find attendance with id: {attendance_id}, because it does not exist or you do not have permission", HttpStatusCode.NotFound));
        }

        /// <summary>
        /// Delete multiple attendance by Id
        /// </summary>
        /// <param name="attendance_id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteMany([FromQuery] long[] attendance_id)
        {
            if (attendance_id == null || attendance_id.Length < 1)
            {
                return BadRequest(Logger.Error($"Empty query params - attendance_id(s) must be passed as query parameter e.g : attendance_id=1&attendance_id=2 for deletion"));

            }
            if (attendance_id.Length > 50)
            {
                return BadRequest(Logger.Error($"Cannot delete more then 50 attendance(s) per request."));
            }

            var affectedRows = await _attendanceService.DeleteAttendanceAsync(attendance_id, org_id);
            if (affectedRows > 0)
            {
                return Ok(Logger.Success($"{affectedRows} attendance(s) with id(s): {string.Join(",", attendance_id)} deleted successfully"));
            }
            return NotFound(Logger.Error($"Cannot find attendance(s) with id(s): {string.Join(",", attendance_id)}, because it does not exist or you do not have permission", HttpStatusCode.NotFound));
        }
    }
}
