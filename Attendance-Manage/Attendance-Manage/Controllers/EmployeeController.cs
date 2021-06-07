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
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Employee employee)
        {
            if (employee == null || !ModelState.IsValid)
                return BadRequest(new { message = "Invalid request body", status_code = HttpStatusCode.BadRequest });

            var emp_id = await _employeeService.CreateEmployee(employee);

            employee.emp_id = emp_id;
            return Ok(employee);
        }
    }
}
