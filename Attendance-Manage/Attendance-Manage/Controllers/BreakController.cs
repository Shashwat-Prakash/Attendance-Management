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
        public BreakController(IBreakService breakService)
        {
            _breakService = breakService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Break attendance_break)
        {
            if (attendance_break == null || !ModelState.IsValid)
                return BadRequest(new { message = "Invalid request body", status_code = HttpStatusCode.BadRequest });

            var break_id = await _breakService.CreateBreak(attendance_break);

            attendance_break.break_id = break_id;
            return Ok(attendance_break);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var _break = await _breakService.GetBreakAsync(id);
            if (_break != null)
                return Ok(_break);
            return NotFound(new { message = "No break found", status_code = HttpStatusCode.NotFound });
        }
    }
}
