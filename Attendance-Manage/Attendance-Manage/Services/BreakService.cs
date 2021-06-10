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
        Task<long> CreateBreakAsync(Break _break);
        Task<Break> GetBreakByBreakIdAsync(long id, long org_id);
        Task<IEnumerable<Break>> GetBreakByAttendanceIdAsync(long id, long org_id);
        Task<IEnumerable<dynamic>> GetBreakByOrgIdAsync(long org_id, Paged paged, IDictionary<string, string> filter);
        Task<bool> UpdateBreakByIdAsync(Break _break);
        Task<int> GetBreakCountAsync(long org_id, IDictionary<string, string> filters);
        Task<int> DeleteBreakAsync(long[] idToDelete, long org_id);
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

        public async Task<long> CreateBreakAsync(Break _break)
        {
            using MySqlConnection connection = new MySqlConnection(_writerDbConnection);
            const string sqlQuery = @"Insert Into Break (attendance_id, user_id, org_id, break_time_in, break_time_out, type)
                    Values(@attendance_id, @user_id, @org_id, @break_time_in, @break_time_out, @type); Select LAST_INSERT_ID(); ";
            long id = await connection.ExecuteScalarAsync<long>(sqlQuery, _break);
            return id;
        }

        public async Task<Break> GetBreakByBreakIdAsync(long break_id, long org_id)
        {
            using MySqlConnection connection = new MySqlConnection(_writerDbConnection);
            const string sqlQuery = @"Select break_id, attendance_id, user_id, org_id, break_time_in, break_time_out, type, created_at, updated_at
                    from Break where break_id = @break_id and org_id = @org_id";

            var _break = await connection.QueryFirstOrDefaultAsync<Break>(sqlQuery, new { break_id, org_id });
            return _break;
        }

        public async Task<IEnumerable<Break>> GetBreakByAttendanceIdAsync(long attendance_id, long org_id)
        {
            using MySqlConnection connection = new MySqlConnection(_writerDbConnection);
            const string sqlQuery = @"Select break_id, attendance_id, user_id, org_id, break_time_in, break_time_out, type, created_at, updated_at
                    from Break where attendance_id = @attendance_id and org_id = @org_id";

            var _break = await connection.QueryAsync<Break>(sqlQuery, new { attendance_id, org_id });
            return _break.ToList();
        }

        public async Task<IEnumerable<dynamic>> GetBreakByOrgIdAsync(long org_id, Paged paged, IDictionary<string, string> filter)
        {
            using MySqlConnection connection = new MySqlConnection(_writerDbConnection);
            string sqlQuery = $@"Select break_id, attendance_id, user_id, org_id, break_time_in, break_time_out, type, created_at, updated_at
                    from Break /**where**/
                    Order by {paged.sort} {paged.order} LIMIT {paged.offset}, {paged.limit};";

            var sql = DynamicSqlExtension.FilterBuilder<Attendance>(sqlQuery, org_id, filter);
            return await connection.QueryAsync(sql.RawSql, sql.Parameters);
        }

        public async Task<bool> UpdateBreakByIdAsync(Break _break)
        {
            using MySqlConnection connection = new MySqlConnection(_writerDbConnection);
            const string sqlQuery = @"Update Break Set break_time_in = @break_time_in, break_time_out = @break_time_out, type = @type                    
                    where break_id = @break_id and org_id = @org_id;";
            var rowAffected = await connection.ExecuteAsync(sqlQuery, _break);
            return rowAffected > 0;
        }

        public async Task<int> GetBreakCountAsync(long org_id, IDictionary<string, string> filters)
        {
            string sqlQuery = @"Select count(1) as total from Break /**where**/ ";
            var dynamicSql = DynamicSqlExtension.FilterBuilder<Attendance>(sqlQuery, org_id, filters);

            using MySqlConnection connection = new MySqlConnection(_readerDbConnection);
            int total = await connection.ExecuteScalarAsync<int>(dynamicSql.RawSql, dynamicSql.Parameters);

            return total;
        }

        public async Task<int> DeleteBreakAsync(long[] idToDelete, long org_id)
        {
            using MySqlConnection connection = new MySqlConnection(_writerDbConnection);
            const string sqlQuery = @"Delete from Break where break_id = @break_id and org_id = @org_id";
            var rowAffected = await connection.ExecuteAsync(sqlQuery,
                idToDelete.Select(x => new
                {
                    break_id = x,
                    org_id
                }));
            return rowAffected;
        }
    }
}
