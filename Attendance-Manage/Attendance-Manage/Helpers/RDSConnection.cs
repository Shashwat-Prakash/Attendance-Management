using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Attendance_Manage.Helpers
{
    public static class RDSConnection
    {
        public static string GetDbConnectionString(IConfiguration config, string rdsDatabaseName = "Test", bool writer = true)
        {
            var rds = config.GetSection("RDS").Get<RDSConfig>();
            if (writer)
            {
                return $"Server={rds.WriterServer};port={rds.Port};Database={rdsDatabaseName};Uid={rds.UserName};Pwd={rds.Password};";
            }
            else
            {
                return $"Server={rds.ReaderServer};port={rds.Port};Database={rdsDatabaseName};Uid={rds.UserName};Pwd={rds.Password};";
            }
        }
    }

    public class RDSConfig
    {
        public string ReaderServer { get; set; }
        public string WriterServer { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Port { get; set; }
    }
}
