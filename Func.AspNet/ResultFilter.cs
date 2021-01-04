namespace Func.AspNet
{
#if NETSTANDARD
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
#else
    using System.Threading;
    using System.Threading.Tasks;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;
#endif
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Reflection;

    public class ResultFilter : IActionFilter
    {
        private readonly ResultConversionConfiguration _configuration;

        private readonly IDictionary<Type, Func<ResultError, ResponseDetails, ErrorResponse>> _errorResponseFactories =
            new Dictionary<Type, Func<ResultError, ResponseDetails, ErrorResponse>>();

        public ResultFilter(ResultConversionConfiguration configuration)
        {
            _configuration = configuration;
        }

#if NETSTANDARD
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (ResponseIsResultType(context.Result, out var result))
                context.Result = GetActionResultForResult(context, result);
            else if (context.Exception != null && !context.ExceptionHandled)
                UpdateActionResultForException(context);
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
        }

        private static bool ResponseIsResultType(IActionResult response, out Result result)
        {
            if (response is ObjectResult o && o.Value is Result r)
            {
                result = r;
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        private IActionResult GetActionResultForResult(ActionExecutedContext context, Result result)
        {
            switch (result)
            {
                case Success s:
                    return GetSuccessResult(context, s);
                case Failure f:
                    return GetFailureResult(context, f);
                default:
                    return new StatusCodeResult(500);
            }
        }

        private void UpdateActionResultForException(ActionExecutedContext context)
        {
            if (!(_configuration.ExceptionResponseConverter is Some<IExceptionResponseConverter> exceptionConverter))
                return;

            context.ExceptionHandled = true;

            context.Result =
                exceptionConverter.Value.GetExceptionResponse(context.Exception)
                    .Map(CreateResponseForErrorResponse);
        }

        private static IActionResult GetSuccessResult(ActionExecutedContext context, Success success)
        {
            IActionResult ResultForSuccess(HttpStatusCode statusCode)
            {
                switch(success.GetValue())
                {
                    case Some<object> s:
                        return new ObjectResult(s.Value) { StatusCode = (int)statusCode };
                    default:
                        return new StatusCodeResult((int)statusCode);
                }
            }

            return (
                GetCustomAttributesForControllerMethod<OnSuccessAttribute>(context.Controller as ControllerBase)
                .SingleOrDefault()
                ?.StatusCode
                ?? HttpStatusCode.OK)
            .Map(ResultForSuccess);
        }

        private IActionResult GetFailureResult(ActionExecutedContext context, Failure failure)
        {
            var error = failure.GetError();
            var errorType = error.GetType();

            void PopulateErrorMessageIfRequired(ResponseDetails response)
            {
                if (string.IsNullOrEmpty(response.Message))
                {
                    response.Message =
                        errorType.GetCustomAttribute<MessageTextSourceAttribute>()
                        ?.SourceName.Map(x => errorType.GetMethod(x) ?? errorType.GetProperty(x).GetGetMethod())
                        ?.Invoke(error, new object[0]) as string
                        ?? string.Empty;
                }
            }

            return (
                    GetCustomAttributesForControllerMethod<OnFailureAttribute>(context.Controller as ControllerBase)
                    ?.FirstOrDefault(x => x.ErrorType.IsAssignableFrom(errorType))?.ResponseDetails
                    ?? errorType.GetCustomAttribute<ProducesStatusCodeAttribute>()?.ResponseDetails
                    ?? new ResponseDetails { StatusCode = HttpStatusCode.InternalServerError }
                )
                .Tee(PopulateErrorMessageIfRequired)
                .Map(GetResponseFactory(errorType), error)
                .Map(CreateResponseForErrorResponse);
        }

        private static IActionResult CreateResponseForErrorResponse(ErrorResponse errorResponse) =>
            new ObjectResult(errorResponse.Body) { StatusCode = (int)errorResponse.StatusCode };

        private static IEnumerable<TAttribute> GetCustomAttributesForControllerMethod<TAttribute>(ControllerBase controller)
            where TAttribute : Attribute
            =>
            controller?.ControllerContext.ActionDescriptor.MethodInfo.GetCustomAttributes<TAttribute>();
#else

        public bool AllowMultiple => false;

        public async Task<HttpResponseMessage> ExecuteActionFilterAsync(HttpActionContext actionContext, CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
        {
            try
            {
                var response = await continuation();

                if (ResponseIsResultType(response, out var result))
                    response = GetHttpResponseForResult(actionContext, result);

                return response;
            }
            catch(Exception exception)
            {
                if (_configuration.ExceptionResponseConverter is Some<IExceptionResponseConverter> exceptionConverter)
                    return
                        exceptionConverter.Value.GetExceptionResponse(exception)
                            .Map(CreateResponseForErrorResponse, actionContext);

                throw;
            }
        }

        private static bool ResponseIsResultType(HttpResponseMessage response, out Result result)
        {
            if (response?.Content is ObjectContent o && o.Value is Result r)
            {
                result = r;
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        private HttpResponseMessage GetHttpResponseForResult(HttpActionContext context, Result result)
        {
            switch (result)
            {
                case Success s:
                    return GetSuccessResult(context, s);
                case Failure f:
                    return GetFailureResult(context, f);
                default:
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        private static HttpResponseMessage GetSuccessResult(HttpActionContext context, Success success)
        {
            HttpResponseMessage ResultForSuccess(HttpStatusCode statusCode) =>
                success.GetValue() is Some<object> s
                    ? context.Request.CreateResponse(statusCode, s.Value)
                    : context.Request.CreateResponse(statusCode);

            return (
                GetCustomAttributesForControllerMethod<OnSuccessAttribute>(context.ControllerContext.Controller as ApiController)
                .SingleOrDefault()
                ?.StatusCode
                ?? HttpStatusCode.OK)
            .Map(ResultForSuccess);
        }

        private HttpResponseMessage GetFailureResult(HttpActionContext context, Failure failure)
        {
            var error = failure.GetError();
            var errorType = error.GetType();

            void PopulateErrorMessageIfRequired(ResponseDetails response)
            {
                if (string.IsNullOrEmpty(response.Message))
                {
                    response.Message =
                        errorType.GetCustomAttribute<MessageTextSourceAttribute>()
                        ?.SourceName.Map(x => errorType.GetMethod(x) ?? errorType.GetProperty(x).GetGetMethod())
                        ?.Invoke(error, new object[0]) as string
                        ?? string.Empty;
                }
            }

            return (
                    GetCustomAttributesForControllerMethod<OnFailureAttribute>(context.ControllerContext.Controller as ApiController)
                    ?.FirstOrDefault(x => x.ErrorType.IsAssignableFrom(errorType))?.ResponseDetails
                    ?? errorType.GetCustomAttribute<ProducesStatusCodeAttribute>()?.ResponseDetails
                    ?? new ResponseDetails { StatusCode = HttpStatusCode.InternalServerError }
                )
                .Tee(PopulateErrorMessageIfRequired)
                .Map(GetResponseFactory(errorType), error)
                .Map(CreateResponseForErrorResponse, context);
        }

        private static HttpResponseMessage CreateResponseForErrorResponse(HttpActionContext context, ErrorResponse errorResponse) =>
            context.Request.CreateResponse(errorResponse.StatusCode, errorResponse.Body);

        private static IEnumerable<TAttribute> GetCustomAttributesForControllerMethod<TAttribute>(ApiController controller)
            where TAttribute : Attribute
            =>
            controller?.ActionContext.ActionDescriptor.GetCustomAttributes<TAttribute>();
#endif

        private Func<ResultError, ResponseDetails, ErrorResponse> GetResponseFactory(Type errorType) =>
            _errorResponseFactories.ContainsKey(errorType)
                ? _errorResponseFactories[errorType]
                : _errorResponseFactories[errorType] =
                    typeof(IErrorResponseConverter)
                        .GetMethod(nameof(IErrorResponseConverter.GetErrorResponse))
                        ?.MakeGenericMethod(errorType)
                        .Map<MethodInfo, Func<ResultError, ResponseDetails, ErrorResponse>>(
                            m => (e, d) =>
                                (ErrorResponse)m.Invoke(_configuration.ErrorResponseConverter, new object[] { e, d }));
    }
}
