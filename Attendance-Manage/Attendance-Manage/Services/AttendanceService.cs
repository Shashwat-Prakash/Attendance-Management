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
    public interface IAttendanceService
    {
        Task<long> CreateAttendance(Attendance attendance);
        Task<Attendance> GetAttendanceAsync(long id);
    }

    public class AttendanceService : IAttendanceService
    {
        private readonly string _readerDbConnection;
        private readonly string _writerDbConnection;
        public AttendanceService(IConfiguration config)
        {
            _writerDbConnection = RDSConnection.GetDbConnectionString(config, writer: true);
            _readerDbConnection = RDSConnection.GetDbConnectionString(config, writer: false);
        }
        public async Task<long> CreateAttendance(Attendance attendance)
        {
            using MySqlConnection connection = new MySqlConnection(_writerDbConnection);
            const string sqlQuery = @"Insert Into Attendance (emp_id, time_in, time_out, current_day, hours, delay,
                    status, last_time_in, last_time_out)
                    Values(@emp_id, @time_in, @time_out, @current_day, @hours, @delay,
                    @status, @last_time_in, @last_time_out); Select LAST_INSERT_ID(); ";
            long id = await connection.ExecuteScalarAsync<long>(sqlQuery, attendance);
            return id;
        }

        public async Task<Attendance> GetAttendanceAsync(long id)
        {
            using MySqlConnection connection = new MySqlConnection(_writerDbConnection);
            const string sqlQuery = @"Select attendance_id, emp_id, time_in, time_out, current_day, hours, delay,
                    status, last_time_in, last_time_out from Attendance where attendance_id = @id";

            var attendance = await connection.QueryFirstOrDefaultAsync<Attendance>(sqlQuery, new { id });            
            return attendance;
        }
    }
}
