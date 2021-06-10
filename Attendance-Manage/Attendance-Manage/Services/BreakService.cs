using Attendance_Manage.Helpers;
using Attendance_Manage.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
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
        Task<IEnumerable<Break>> GetBreakAsync(long id, long org_id);
        Task<IEnumerable<Break>> GetBreakAsyncByAttendanceId(long id, long org_id);
    }

    public class BreakService : IBreakService
    {
        private readonly string _readerDbConnection;
        private readonly string _writerDbConnection;
        public BreakService(IConfiguration config)
        {
            _writerDbConnection = RDSConnection.GetDbConnectionString(config, writer: true);
            _readerDbConnection = RDSConnection.GetDbConnectionString(config, writer: false);
        }

        public async Task<long> CreateBreak(Break attendance_break)
        {
            using MySqlConnection connection = new MySqlConnection(_writerDbConnection);
            const string sqlQuery = @"Insert Into Break (attendance_id, user_id, org_id, break_time_in, break_time_out, type)
                    Values(@attendance_id, @user_id, @org_id, @break_time_in, @break_time_out, @type); Select LAST_INSERT_ID(); ";
            long id = await connection.ExecuteScalarAsync<long>(sqlQuery, attendance_break);
            return id;
        }

        public async Task<IEnumerable<Break>> GetBreakAsync(long break_id, long org_id)
        {
            using MySqlConnection connection = new MySqlConnection(_writerDbConnection);
            const string sqlQuery = @"Select break_id, attendance_id, user_id, org_id, break_time_in, break_time_out, type 
                    from Break where break_id = @break_id and org_id = @org_id";

            var _break = await connection.QueryAsync<Break>(sqlQuery, new { break_id, org_id });
            return _break.ToList();
        }

        public async Task<IEnumerable<Break>> GetBreakAsyncByAttendanceId(long break_id, long org_id)
        {
            using MySqlConnection connection = new MySqlConnection(_writerDbConnection);
            const string sqlQuery = @"Select break_id, attendance_id, user_id, org_id, break_time_in, break_time_out, type
                    from Break where attendance_id = @break_id and org_id = @org_id";

            var _break = await connection.QueryAsync<Break>(sqlQuery, new { break_id, org_id });
            return _break.ToList();
        }
    }
}
