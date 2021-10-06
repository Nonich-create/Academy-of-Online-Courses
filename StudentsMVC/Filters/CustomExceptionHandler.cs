using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using System;
using System.Net;

namespace Students.MVC.Filters
{
    public class CustomExceptionHandler : IExceptionFilter
    {
        private readonly IModelMetadataProvider _modelMetadataProvider;

        public CustomExceptionHandler(IModelMetadataProvider modelMetadataProvider, IConfiguration config)
        {
            _modelMetadataProvider = modelMetadataProvider;
        }
        public void OnException(ExceptionContext context)
        {
            HttpStatusCode statusCode = (context.Exception as WebException != null &&
                        ((HttpWebResponse)(context.Exception as WebException).Response) != null) ?
                         ((HttpWebResponse)(context.Exception as WebException).Response).StatusCode
                         : GetErrorCode(context.Exception.GetType());
            var result = new ViewResult { ViewName = "Error",StatusCode = (int)statusCode };
            result.ViewData = new ViewDataDictionary(_modelMetadataProvider, context.ModelState);
            context.ExceptionHandled = true;
            result.ViewData.Add("Exception", $"{(int)statusCode}");
            result.ViewData.Add("ExceptionInfo", $"Message: {context.Exception} <br/><br/> StackTrace: {context.Exception.StackTrace}");
            result.ViewData.Add("Environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));
            context.Result = result;
        }
        private HttpStatusCode GetErrorCode(Type exceptionType)
        {
            if (Enum.TryParse(exceptionType.Name, out Exceptions tryParseResult))
            {
                return tryParseResult switch
                {
                    Exceptions.NullReferenceException => HttpStatusCode.LengthRequired,
                    Exceptions.FileNotFoundException => HttpStatusCode.NotFound,
                    Exceptions.OverflowException => HttpStatusCode.RequestedRangeNotSatisfiable,
                    Exceptions.OutOfMemoryException => HttpStatusCode.ExpectationFailed,
                    Exceptions.InvalidCastException => HttpStatusCode.PreconditionFailed,
                    Exceptions.ObjectDisposedException => HttpStatusCode.Gone,
                    Exceptions.UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                    Exceptions.NotImplementedException => HttpStatusCode.NotImplemented,
                    Exceptions.NotSupportedException => HttpStatusCode.NotAcceptable,
                    Exceptions.InvalidOperationException => HttpStatusCode.MethodNotAllowed,
                    Exceptions.TimeoutException => HttpStatusCode.RequestTimeout,
                    Exceptions.ArgumentException => HttpStatusCode.BadRequest,
                    Exceptions.StackOverflowException => HttpStatusCode.RequestedRangeNotSatisfiable,
                    Exceptions.FormatException => HttpStatusCode.UnsupportedMediaType,
                    Exceptions.IOException => HttpStatusCode.NotFound,
                    Exceptions.IndexOutOfRangeException => HttpStatusCode.ExpectationFailed,
                    Exceptions.SqlException => HttpStatusCode.InternalServerError,
                    _ => HttpStatusCode.InternalServerError,
                };
            }
            else
            {
                return HttpStatusCode.InternalServerError;
            }
        }
    }
}   
