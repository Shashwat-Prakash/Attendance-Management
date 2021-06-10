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
        Task<Attendance> GetAttendanceAsync(long attendance_id, long org_id);
        Task<bool> UpdateAttendanceAsync(Attendance attendance);
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
            const string sqlQuery = @"Insert Into Attendance (user_id, org_id, time_in)
                    Values(@user_id, @org_id, @time_in); Select LAST_INSERT_ID(); ";
            long id = await connection.ExecuteScalarAsync<long>(sqlQuery, attendance);
            return id;
        }

        public async Task<Attendance> GetAttendanceAsync(long attendance_id, long org_id)
        {
            using MySqlConnection connection = new MySqlConnection(_writerDbConnection);
            const string sqlQuery = @"Select attendance_id, user_id, org_id, time_in, time_out
                    from Attendance where attendance_id = @attendance_id and org_id = @org_id";

            var attendance = await connection.QueryFirstOrDefaultAsync<Attendance>(sqlQuery, new { attendance_id, org_id });            
            return attendance;
        }

        public async Task<bool> UpdateAttendanceAsync(Attendance attendance)
        {
            using MySqlConnection connection = new MySqlConnection(_writerDbConnection);
            const string sqlQuery = @"Update Attendance Set time_in = @time_in, time_out = @time_out                     
                    where attendance_id = @attendance_id and org_id = @org_id;";
            var rowAffected = await connection.ExecuteAsync(sqlQuery, attendance);
            return rowAffected > 0;
        }
    }
}
