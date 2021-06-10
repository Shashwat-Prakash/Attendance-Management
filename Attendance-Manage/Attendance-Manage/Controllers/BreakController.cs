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
    public class BreakController : ControllerBase
    {
        private readonly IBreakService _breakService;
        long org_id = 3;
        public BreakController(IBreakService breakService)
        {
            _breakService = breakService;
        }

        /// <summary>
        /// Create a break
        /// </summary>
        /// <param name="attendance_break"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Break attendance_break)
        {
            if (attendance_break == null || !ModelState.IsValid)
                return BadRequest(Logger.Error("Invalid request body"));

            var break_id = await _breakService.CreateBreakAsync(attendance_break);

            attendance_break.break_id = break_id;
            return Ok(attendance_break);
        }

        /// <summary>
        /// Get break by Id
        /// </summary>
        /// <param name="break_id"></param>
        /// <returns></returns>
        [HttpGet("{break_id}")]
        public async Task<IActionResult> Get(long break_id)
        {
            var _break = await _breakService.GetBreakByBreakIdAsync(break_id, org_id);
            if (_break != null)
                return Ok(_break);
            return NotFound(Logger.Error("No break found", HttpStatusCode.NotFound));
        }

        /// <summary>
        /// Get all break
        /// </summary>
        /// <param name="paged"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] Paged paged, [FromQuery] IDictionary<string, string> filters)
        {
            paged.sort ??= "break_id";

            var _break = await _breakService.GetBreakByOrgIdAsync(org_id, paged, filters);
            if (_break != null && _break.Any())
            {
                var table = new TableResult<dynamic>
                {
                    limit = paged.limit,
                    offset = paged.offset,
                    returned = _break.Count(),
                    total = await _breakService.GetBreakCountAsync(org_id, filters),
                    result = _break
                };
                return Ok(table);
            }
            return NotFound(Logger.Error($"No break found", HttpStatusCode.NotFound));
        }

        /// <summary>
        /// Update break by Id
        /// </summary>
        /// <param name="_break"></param>
        /// <param name="break_id"></param>
        /// <returns></returns>
        [HttpPut("{break_id}")]
        public async Task<IActionResult> Update([FromBody] Break _break, long break_id)
        {
            if (_break == null || !ModelState.IsValid)
            {
                return BadRequest(Logger.Error("Invalid request body"));
            }

            var _breakResult = await _breakService.GetBreakByBreakIdAsync(break_id, org_id);

            _break.break_id = break_id;
            if (_break.break_time_out != null)
            {
                _break.break_time_in = _breakResult.break_time_in;
            }

            else
            {
                _break.break_time_out = _breakResult.break_time_out;
            }
            var isUpdated = await _breakService.UpdateBreakByIdAsync(_break);
            if (isUpdated)
                return Ok(Logger.Success($"Break with id: {break_id} updated succesfully"));

            return StatusCode((int)HttpStatusCode.InternalServerError, Logger.ServerError("Internal server, please try again!"));
        }

        /// <summary>
        /// Delete break by Id
        /// </summary>
        /// <param name="break_id"></param>
        /// <returns></returns>
        [HttpDelete("{break_id}")]
        public async Task<IActionResult> Delete(long break_id)
        {
            if (break_id < 1)
            {
                return BadRequest(Logger.Error("Invalid break id", HttpStatusCode.BadRequest));
            }

            var idsToDelete = new long[] { break_id };
            var rowAffected = await _breakService.DeleteBreakAsync(idsToDelete, org_id);
            if (rowAffected > 0)
            {
                return Ok(Logger.Success($"Break with id: {break_id} deleted successfully"));
            }

            return NotFound(Logger.Error($"Cannot find break with id: {break_id}, because it does not exist or you do not have permission", HttpStatusCode.NotFound));
        }

        /// <summary>
        /// Delete multiple break by Id
        /// </summary>
        /// <param name="break_id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteMany([FromQuery] long[] break_id)
        {
            if (break_id == null || break_id.Length < 1)
            {
                return BadRequest(Logger.Error($"Empty query params - break_id(s) must be passed as query parameter e.g : break_id=1&break_id=2 for deletion"));

            }
            if (break_id.Length > 50)
            {
                return BadRequest(Logger.Error($"Cannot delete more then 50 break(s) per request."));
            }

            var affectedRows = await _breakService.DeleteBreakAsync(break_id, org_id);
            if (affectedRows > 0)
            {
                return Ok(Logger.Success($"{affectedRows} break(s) with id(s): {string.Join(",", break_id)} deleted successfully"));
            }
            return NotFound(Logger.Error($"Cannot find break(s) with id(s): {string.Join(",", break_id)}, because it does not exist or you do not have permission", HttpStatusCode.NotFound));
        }
    }
}
