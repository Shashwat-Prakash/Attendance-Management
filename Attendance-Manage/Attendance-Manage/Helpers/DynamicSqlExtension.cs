using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Attendance_Manage.Helpers
{
    public static class DynamicSqlExtension
    {
		public static SqlBuilder.Template FilterBuilder<T>(string sqlQuery, long org_id, IDictionary<string, string> filters = null, string orgSqlProperty = "org_id")
		{
			var builder = new SqlBuilder();

			// Add org_id to ensure cross-account access is not possible in dynamic where
			// Use orgSqlProperty to allow dynamic name for join for example - tableA.org_id = @org_id
			builder.Where($"{orgSqlProperty} = @org_id", new { org_id });

			var parameters = new DynamicParameters();
			var query = builder.AddTemplate(sqlQuery, parameters);

			// Return if null
			if (filters == null || filters.Count < 1) { return query; }

			// Add dynamic where clause/params to allow table filter on SQL level
			// Using paramater to prevent SQL injection attack
			foreach (var filter in filters)
			{
				var property = typeof(T).GetProperty(filter.Key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
				if (property != null) // Ensure field exits in SQL
				{
					var sqlFilter = GetSqlFilter(filter.Value);

					// Null filter
					if (sqlFilter.Condition.Equals("Nu", StringComparison.OrdinalIgnoreCase))
					{
						builder.Where($"{filter.Key} is null");
					}
					// Not null filter
					else if (sqlFilter.Condition.Equals("Nn", StringComparison.OrdinalIgnoreCase))
					{
						builder.Where($"{filter.Key} is not null");
					}
					else
					{
						// Date filter
						if (property.PropertyType == typeof(DateTime?) || property.PropertyType == typeof(DateTime))
						{
							builder.Where($"cast({filter.Key} as date) {sqlFilter.Condition} date(@{filter.Key})"); // Case to date for date filter
							parameters.Add(filter.Key, sqlFilter.Value); // Must add as paramater to prevent SQL injection attack
						}
						// String filter
						else
						{
							builder.Where($"{filter.Key} {sqlFilter.Condition} @{filter.Key}");
							parameters.Add(filter.Key, sqlFilter.Value); // Must add as paramater to prevent SQL injection attack
						}
					}
				}
			}
			return query;
		}

		private static SqlFilter GetSqlFilter(string value)
		{
			// Extract operator name for eg. 'name=eq:vikash' or 'Gt:10'
			var sqlFilter = new SqlFilter
			{
				Condition = value.Split(':').FirstOrDefault()?.Trim(),
				Value = value.Split(':').LastOrDefault()?.Trim()
			};

			// Supported conditions
			var operatorsList = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
			{
				{ "Eq", "="},
				{ "Ne", "<>"},
				{ "Gt", ">"},
				{ "Ge", ">="},
				{ "Lt", "<"},
				{ "Le", "<="},
			};

			sqlFilter.Condition = operatorsList.TryGetValue(sqlFilter.Condition, out string output) ? output : "=";

			return sqlFilter;
		}
	}

	public class SqlFilter
	{
		public string Condition { get; set; }
		public string Value { get; set; }
	}
}
