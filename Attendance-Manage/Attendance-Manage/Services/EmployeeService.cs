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
    public interface IEmployeeService
    {
        Task<long> CreateEmployee(Employee employee);
    }

    public class EmployeeService : IEmployeeService
    {
        private readonly string _readerDbConnection;
        private readonly string _writerDbConnection;
        public EmployeeService(IConfiguration config)
        {
            _writerDbConnection = RDSConnection.GetDbConnectionString(config, writer: true);
            _readerDbConnection = RDSConnection.GetDbConnectionString(config, writer: false);
        }

        public async Task<long> CreateEmployee(Employee employee)
        {
            using MySqlConnection connection = new MySqlConnection(_writerDbConnection);
            const string sqlQuery = @"Insert Into Employee (user_id, org_id, name, contact, email_id, dept_name, designation_name,
                    salary, joining_date, type)
                    Values(@user_id, @org_id, @name, @contact, @email_id, @dept_name, @designation_name,
                    @salary, @joining_date, @type); Select LAST_INSERT_ID(); ";
            long id = await connection.ExecuteScalarAsync<long>(sqlQuery, employee);
            return id;
        }
    }
}
