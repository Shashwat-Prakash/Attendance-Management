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
        Task<long> CreateAttendanceAsync(Attendance attendance);
        Task<Attendance> GetAttendanceByIdAsync(long attendance_id, long org_id);
        Task<IEnumerable<dynamic>> GetAttendanceByOrgIdAsync(long org_id, Paged paged, IDictionary<string, string> filter);
        Task<bool> UpdateAttendanceByIdAsync(Attendance attendance);
        Task<int> DeleteAttendanceAsync(long[] idToDelete, long org_id);
        Task<int> GetAttendanceCountAsync(long org_id, IDictionary<string, string> filters);
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

        public async Task<long> CreateAttendanceAsync(Attendance attendance)
        {
            using MySqlConnection connection = new MySqlConnection(_writerDbConnection);
            const string sqlQuery = @"Insert Into Attendance (user_id, org_id, time_in)
                    Values(@user_id, @org_id, @time_in); Select LAST_INSERT_ID(); ";
            long id = await connection.ExecuteScalarAsync<long>(sqlQuery, attendance);
            return id;
        }

        public async Task<Attendance> GetAttendanceByIdAsync(long attendance_id, long org_id)
        {
            using MySqlConnection connection = new MySqlConnection(_writerDbConnection);
            const string sqlQuery = @"Select attendance_id, user_id, org_id, time_in, time_out, created_at, updated_at
                    from Attendance where attendance_id = @attendance_id and org_id = @org_id";

            var attendance = await connection.QueryFirstOrDefaultAsync<Attendance>(sqlQuery, new { attendance_id, org_id });            
            return attendance;
        }

        public async Task<IEnumerable<dynamic>> GetAttendanceByOrgIdAsync(long org_id, Paged paged, IDictionary<string, string> filter)
        {
            using MySqlConnection connection = new MySqlConnection(_writerDbConnection);
            string sqlQuery = $@"Select attendance_id, user_id, org_id, time_in, time_out, created_at, updated_at
                    from Attendance /**where**/
                    Order by {paged.sort} {paged.order} LIMIT {paged.offset}, {paged.limit};";

            var sql = DynamicSqlExtension.FilterBuilder<Attendance>(sqlQuery, org_id, filter);
            return await connection.QueryAsync(sql.RawSql, sql.Parameters);
        }

        public async Task<bool> UpdateAttendanceByIdAsync(Attendance attendance)
        {
            using MySqlConnection connection = new MySqlConnection(_writerDbConnection);
            const string sqlQuery = @"Update Attendance Set time_in = @time_in, time_out = @time_out                     
                    where attendance_id = @attendance_id and org_id = @org_id;";
            var rowAffected = await connection.ExecuteAsync(sqlQuery, attendance);
            return rowAffected > 0;
        }

        public async Task<int> GetAttendanceCountAsync(long org_id, IDictionary<string, string> filters)
        {
            string sqlQuery = @"Select count(1) as total from Attendance /**where**/ ";
            var dynamicSql = DynamicSqlExtension.FilterBuilder<Attendance>(sqlQuery, org_id, filters);

            using MySqlConnection connection = new MySqlConnection(_readerDbConnection);
            int total = await connection.ExecuteScalarAsync<int>(dynamicSql.RawSql, dynamicSql.Parameters);            

            return total;
        }

        public async Task<int> DeleteAttendanceAsync(long[] idToDelete, long org_id)
        {            
            using MySqlConnection connection = new MySqlConnection(_writerDbConnection);
            const string sqlQuery = @"Delete from Attendance where attendance_id = @attendance_id and org_id = @org_id";
            var rowAffected = await connection.ExecuteAsync(sqlQuery,
                idToDelete.Select(x => new
                {
                    attendance_id = x,
                    org_id
                }));
            return rowAffected;
        }
    }
}
