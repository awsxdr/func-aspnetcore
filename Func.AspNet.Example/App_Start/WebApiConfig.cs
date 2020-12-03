namespace Func.AspNet.Example
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Net;
    using System.Runtime.Serialization;
    using System.Web.Http;

    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.AddResultConversion(c => c
                .WithExceptionHandler(new TestExceptionResponseConverter())
                .WithErrorResponseConverter(new TestErrorResponseConverter()));
        }

    }

    public class TestExceptionResponseConverter : IExceptionResponseConverter
    {
        public ErrorResponse GetExceptionResponse(Exception exception) =>
            new ErrorResponse
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Body = new
                {
                    Message = $"Oops! An error occurred: {exception.GetType().Name}",
                    ExtraDetails = (exception is ValidationException ve) ? ve.Value : null,
                }
            };
    }

    public class TestErrorResponseConverter : IErrorResponseConverter
    {
        public ErrorResponse GetErrorResponse<TError>(TError error, ResponseDetails configuredResponseDetails)
            where TError : ResultError =>
            new ErrorResponse
            {
                StatusCode = HttpStatusCode.NotImplemented,
                Body = new TestErrorResponseData
                {
                    TestField = "This is a test",
                    Message = configuredResponseDetails.Message,
                    ActualStatusCode = (int)configuredResponseDetails.StatusCode
                }
            };

        [DataContract]
        public class TestErrorResponseData
        {
            [DataMember]
            public string TestField { get; set; }

            [DataMember]
            public string Message { get; set; }

            [DataMember]
            public int ActualStatusCode { get; set; }
        }
    }
}
