using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Attendance_Manage.Helpers
{
    public class Logger
    {
        internal static CustomError ValidationError(IList<ValidationFailure> ErrorList, HttpStatusCode status = HttpStatusCode.BadRequest)
        {
            var errors = new List<ErrorDetails>();
            foreach (var e in ErrorList)
            {
                errors.Add(new ErrorDetails
                {
                    property_name = e.PropertyName,
                    error_message = e.ErrorMessage
                });
            }

            var result = new CustomError
            {
                message = $"{errors.Count} validation error. See 'details' property for more information.",
                details = errors,
                status_code = (int)status
            };
            return result;
        }

        internal static CustomError Error(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            return new CustomError
            {
                message = message,
                status_code = (int)statusCode
            };
        }

        internal static CustomError ServerError(string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        {
            // Logging - To be implemented

            return new CustomError
            {
                message = message,
                status_code = (int)statusCode
            };
        }

        internal static CustomSuccess Success(string message, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return new CustomSuccess
            {
                message = message,
                status_code = (int)statusCode
            };
        }
    }

    public class CustomError
    {
        public int status_code { get; set; }
        public string message { get; set; }
        public List<ErrorDetails> details { get; set; }
    }

    public class ErrorDetails
    {
        public string property_name { get; set; }
        public string error_message { get; set; }
    }

    public class CustomSuccess
    {
        public int status_code { get; set; }
        public string message { get; set; }
    }
}
