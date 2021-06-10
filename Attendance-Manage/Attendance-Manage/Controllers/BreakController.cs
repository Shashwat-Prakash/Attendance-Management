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

            var break_id = await _breakService.CreateBreak(attendance_break);

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
            var _break = await _breakService.GetBreakAsync(break_id, org_id);
            if (_break != null)
                return Ok(_break);
            return NotFound(Logger.Error("No break found", HttpStatusCode.NotFound));
        }
    }
}
