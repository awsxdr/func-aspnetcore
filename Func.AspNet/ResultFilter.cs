namespace Func.AspNet
{
#if NETSTANDARD
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
#else
    using System.Threading;
    using System.Threading.Tasks;
    using System.Net.Http;
    using System.Net.Http.Formatting;
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
#if NETSTANDARD
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if(ResponseIsResultType(context.Result, out var result))
                context.Result = GetActionResultForResult(context, result);
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

        private static IActionResult GetActionResultForResult(ActionExecutedContext context, Result result)
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

        private static IActionResult GetSuccessResult(ActionExecutedContext context, Success success)
        {
            IActionResult GetResultForSuccess(HttpStatusCode statusCode)
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
            .Map(GetResultForSuccess);
        }

        private static IActionResult GetFailureResult(ActionExecutedContext context, Failure failure)
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
                .Map(x => string.IsNullOrEmpty(x.Message) 
                    ? (IActionResult) new StatusCodeResult((int)x.StatusCode)
                    : new ObjectResult(x.Message) { StatusCode = (int)x.StatusCode });
        }

        private static IEnumerable<TAttribute> GetCustomAttributesForControllerMethod<TAttribute>(ControllerBase controller)
            where TAttribute : Attribute
            =>
            controller?.ControllerContext.ActionDescriptor.MethodInfo.GetCustomAttributes<TAttribute>();
#else

        public bool AllowMultiple => false;

        public async Task<HttpResponseMessage> ExecuteActionFilterAsync(HttpActionContext actionContext, CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
        {
            var response = await continuation();

            if (ResponseIsResultType(response, out var result))
                response = GetHttpResponseForResult(actionContext, result);

            return response;
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

        private static HttpResponseMessage GetHttpResponseForResult(HttpActionContext context, Result result)
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
            HttpResponseMessage GetResultForSuccess(HttpStatusCode statusCode) =>
                new HttpResponseMessage(statusCode)
                {
                    Content =
                        success.GetValue() is Some<object> s
                        ? new ObjectContent(s.Value.GetType(), s.Value, context.ControllerContext.Configuration.Formatters.JsonFormatter)
                        : null
                };

            return (
                GetCustomAttributesForControllerMethod<OnSuccessAttribute>(context.ControllerContext.Controller as ApiController)
                .SingleOrDefault()
                ?.StatusCode
                ?? HttpStatusCode.OK)
            .Map(GetResultForSuccess);
        }

        private static HttpResponseMessage GetFailureResult(HttpActionContext context, Failure failure)
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
                .Map(x => new HttpResponseMessage(x.StatusCode) { Content = new StringContent(x.Message) });
        }

        private static IEnumerable<TAttribute> GetCustomAttributesForControllerMethod<TAttribute>(ApiController controller)
            where TAttribute : Attribute
            =>
            controller?.ActionContext.ActionDescriptor.GetCustomAttributes<TAttribute>();
#endif
    }
}
