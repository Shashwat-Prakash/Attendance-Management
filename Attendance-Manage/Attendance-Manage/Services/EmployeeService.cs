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
        public EmployeeService(IConfiguration config)
        {
            /*_writerDbConnection = RDSConnection.GetDbConnectionString(config, writer: true);
            _readerDbConnection = RDSConnection.GetDbConnectionString(config, writer: false);      */
        }

        public async Task<long> CreateEmployee(Employee employee)
        {
            using MySqlConnection connection = new MySqlConnection("");
            const string sqlQuery = @"Insert Into Users (emp_id, name, contact, email_id, dept_name, designation_name,
                    salary, joining_date, type)
                    Values(@emp_id, @name, @contact, @email_id, @dept_name, @designation_name,
                    @salary, @joining_date, @type); Select LAST_INSERT_ID(); ";
            long id = await connection.ExecuteScalarAsync<long>(sqlQuery, employee);
            return id;
        }
    }
}
