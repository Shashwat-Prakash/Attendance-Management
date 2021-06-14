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
    public interface IShiftService
    {
        Task<long> CreateShiftAsync(Shift _shift);
        Task<int> DeleteShiftAsync(long[] idToDelete, long org_id);
        Task<IEnumerable<dynamic>> GetShiftByOrgIdAsync(long org_id, Paged paged, IDictionary<string, string> filter);
        Task<Shift> GetShiftByShiftIdAsync(long shift_id, long org_id);
        Task<int> GetShiftCountAsync(long org_id, IDictionary<string, string> filters);
        Task<bool> UpdateShiftByIdAsync(Shift shift);
    }

    public class ShiftService : IShiftService
    {
        private readonly string _readerDbConnection;
        private readonly string _writerDbConnection;
        public ShiftService(IConfiguration config)
        {
            _writerDbConnection = RDSConnection.GetDbConnectionString(config, writer: true);
            _readerDbConnection = RDSConnection.GetDbConnectionString(config, writer: false);
        }

        public async Task<long> CreateShiftAsync(Shift _shift)
        {
            using MySqlConnection connection = new MySqlConnection(_writerDbConnection);
            const string sqlQuery = @"Insert Into Shift (org_id, name, color, shift_start, shift_end)
                    Values(@org_id, @name, @color, @shift_start, @shift_end); Select LAST_INSERT_ID(); ";
            long id = await connection.ExecuteScalarAsync<long>(sqlQuery, _shift);
            return id;
        }

        public async Task<Shift> GetShiftByShiftIdAsync(long shift_id, long org_id)
        {
            using MySqlConnection connection = new MySqlConnection(_writerDbConnection);
            const string sqlQuery = @"Select shift_id, org_id, name, color, shift_start, shift_end, created_at, updated_at
                    from Shift where shift_id = @shift_id and org_id = @org_id";

            var shift = await connection.QueryFirstOrDefaultAsync<Shift>(sqlQuery, new { shift_id, org_id });
            return shift;
        }

        public async Task<IEnumerable<dynamic>> GetShiftByOrgIdAsync(long org_id, Paged paged, IDictionary<string, string> filter)
        {
            using MySqlConnection connection = new MySqlConnection(_writerDbConnection);
            string sqlQuery = $@"Select shift_id, org_id, name, color, shift_start, shift_end, created_at, updated_at
                    from Shift /**where**/
                    Order by {paged.sort} {paged.order} LIMIT {paged.offset}, {paged.limit};";

            var sql = DynamicSqlExtension.FilterBuilder<Shift>(sqlQuery, org_id, filter);
            return await connection.QueryAsync(sql.RawSql, sql.Parameters);
        }

        public async Task<bool> UpdateShiftByIdAsync(Shift shift)
        {
            using MySqlConnection connection = new MySqlConnection(_writerDbConnection);
            const string sqlQuery = @"Update Shift Set name = @name, color = @color, shift_start = @shift_start, shift_end = @shift_end                    
                    where shift_id = @shift_id and org_id = @org_id;";
            var rowAffected = await connection.ExecuteAsync(sqlQuery, shift);
            return rowAffected > 0;
        }

        public async Task<int> GetShiftCountAsync(long org_id, IDictionary<string, string> filters)
        {
            string sqlQuery = @"Select count(1) as total from Shift /**where**/ ";
            var dynamicSql = DynamicSqlExtension.FilterBuilder<Attendance>(sqlQuery, org_id, filters);

            using MySqlConnection connection = new MySqlConnection(_readerDbConnection);
            int total = await connection.ExecuteScalarAsync<int>(dynamicSql.RawSql, dynamicSql.Parameters);

            return total;
        }

        public async Task<int> DeleteShiftAsync(long[] idToDelete, long org_id)
        {
            using MySqlConnection connection = new MySqlConnection(_writerDbConnection);
            const string sqlQuery = @"Delete from Shift where shift_id = @shift_id and org_id = @org_id";
            var rowAffected = await connection.ExecuteAsync(sqlQuery,
                idToDelete.Select(x => new
                {
                    shift_id = x,
                    org_id
                }));
            return rowAffected;
        }
    }
}
