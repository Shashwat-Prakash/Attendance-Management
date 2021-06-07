using Attendance_Manage.Models;
using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Attendance_Manage.Services
{
    public interface IBreakService
    {
        Task<long> CreateBreak(Break attendance_break);
        Task<Break> GetBreakAsync(long id);
    }

    public class BreakService : IBreakService
    {
        public BreakService()
        {

        }

        public async Task<long> CreateBreak(Break attendance_break)
        {
            using MySqlConnection connection = new MySqlConnection("");
            const string sqlQuery = @"Insert Into Break (attendance_id, emp_id, break_time_in, break_time_out, current_date, break_hours, type,
                    status, last_break_time_in, last_break_time_out)
                    Values(@attendance_id, @emp_id, @break_time_in, @break_time_out, @current_date, @break_hours, @type,
                    @status, @last_break_time_in, @last_break_time_out); Select LAST_INSERT_ID(); ";
            long id = await connection.ExecuteScalarAsync<long>(sqlQuery, attendance_break);
            return id;
        }

        public async Task<Break> GetBreakAsync(long id)
        {
            using MySqlConnection connection = new MySqlConnection("");
            const string sqlQuery = @"Select break_id, attendance_id, emp_id, break_time_in, break_time_out, current_date, break_hours, type,
                    status, last_break_time_in, last_break_time_out from Break where break_id = @id";

            var _break = await connection.QueryFirstOrDefaultAsync<Break>(sqlQuery, new { id });
            return _break;
        }
    }
}
