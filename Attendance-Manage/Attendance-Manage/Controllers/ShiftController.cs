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
    public class ShiftController : ControllerBase
    {
        private readonly IShiftService _shiftService;
        long org_id = 3;
        public ShiftController(IShiftService shiftService)
        {
            _shiftService = shiftService;
        }

        /// <summary>
        /// Create a shift
        /// </summary>
        /// <param name="shift"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Shift shift)
        {
            if (shift == null || !ModelState.IsValid)
                return BadRequest(Logger.Error("Invalid request body"));

            //shift.shift_start = default(DateTime).Add(shift.shift_start.TimeOfDay);
            //shift.shift_end.ToShortTimeString();

            var shift_id = await _shiftService.CreateShiftAsync(shift);

            shift.shift_id = shift_id;
            return Ok(shift);
        }

        /// <summary>
        /// Get shift by Id
        /// </summary>
        /// <param name="shift_id"></param>
        /// <returns></returns>
        [HttpGet("{shift_id}")]
        public async Task<IActionResult> Get(long shift_id)
        {
            var shift = await _shiftService.GetShiftByShiftIdAsync(shift_id, org_id);
            if (shift != null)
                return Ok(shift);
            return NotFound(Logger.Error("No shift found", HttpStatusCode.NotFound));
        }

        /// <summary>
        /// Get all shift
        /// </summary>
        /// <param name="paged"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] Paged paged, [FromQuery] IDictionary<string, string> filters)
        {
            paged.sort ??= "shift_id";

            var shift = await _shiftService.GetShiftByOrgIdAsync(org_id, paged, filters);
            if (shift != null && shift.Any())
            {
                var table = new TableResult<dynamic>
                {
                    limit = paged.limit,
                    offset = paged.offset,
                    returned = shift.Count(),
                    total = await _shiftService.GetShiftCountAsync(org_id, filters),
                    result = shift
                };
                return Ok(table);
            }
            return NotFound(Logger.Error($"No shift found", HttpStatusCode.NotFound));
        }

        /// <summary>
        /// Update shift by Id
        /// </summary>
        /// <param name="shift"></param>
        /// <param name="shift_id"></param>
        /// <returns></returns>
        [HttpPut("{shift_id}")]
        public async Task<IActionResult> Update([FromBody] Shift shift, long shift_id)
        {
            if (shift == null || !ModelState.IsValid)
            {
                return BadRequest(Logger.Error("Invalid request body"));
            }

            shift.shift_id = shift_id;
            var isUpdated = await _shiftService.UpdateShiftByIdAsync(shift);
            if (isUpdated)
                return Ok(Logger.Success($"Shift with id: {shift_id} updated succesfully"));

            return StatusCode((int)HttpStatusCode.InternalServerError, Logger.ServerError("Internal server, please try again!"));
        }

        /// <summary>
        /// Delete shift by Id
        /// </summary>
        /// <param name="shift_id"></param>
        /// <returns></returns>
        [HttpDelete("{shift_id}")]
        public async Task<IActionResult> Delete(long shift_id)
        {
            if (shift_id < 1)
            {
                return BadRequest(Logger.Error("Invalid shift id", HttpStatusCode.BadRequest));
            }

            var idsToDelete = new long[] { shift_id };
            var rowAffected = await _shiftService.DeleteShiftAsync(idsToDelete, org_id);
            if (rowAffected > 0)
            {
                return Ok(Logger.Success($"Shift with id: {shift_id} deleted successfully"));
            }

            return NotFound(Logger.Error($"Cannot find shift with id: {shift_id}, because it does not exist or you do not have permission", HttpStatusCode.NotFound));
        }

        /// <summary>
        /// Delete multiple shift by Id
        /// </summary>
        /// <param name="shift_id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteMany([FromQuery] long[] shift_id)
        {
            if (shift_id == null || shift_id.Length < 1)
            {
                return BadRequest(Logger.Error($"Empty query params - shift_id(s) must be passed as query parameter e.g : shift_id=1&shift_id=2 for deletion"));

            }
            if (shift_id.Length > 50)
            {
                return BadRequest(Logger.Error($"Cannot delete more then 50 shift(s) per request."));
            }

            var affectedRows = await _shiftService.DeleteShiftAsync(shift_id, org_id);
            if (affectedRows > 0)
            {
                return Ok(Logger.Success($"{affectedRows} shift(s) with id(s): {string.Join(",", shift_id)} deleted successfully"));
            }
            return NotFound(Logger.Error($"Cannot find shift(s) with id(s): {string.Join(",", shift_id)}, because it does not exist or you do not have permission", HttpStatusCode.NotFound));
        }
    }
}
