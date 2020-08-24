namespace Func.AspNet
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;

    public class ResultFilter : IActionFilter
    {
        public bool AllowMultiple => false;

        public async Task<HttpResponseMessage> ExecuteActionFilterAsync(HttpActionContext actionContext, CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
        {
            var response = await continuation();

            if(ResponseIsResultType(response, out var result))
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
            switch(result)
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
                        ? new ObjectContent(s.Value.GetType(), s.Value, new JsonMediaTypeFormatter())
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
            var errorType = failure.GetError().GetType();

            return
                GetCustomAttributesForControllerMethod<OnFailureAttribute>(context.ControllerContext.Controller as ApiController)
                ?.FirstOrDefault(x => x.ErrorType.IsAssignableFrom(errorType))?.ToResponseMessage()
                ?? errorType.GetCustomAttribute<ProducesStatusCodeAttribute>()?.ToResponseMessage()
                ?? new HttpResponseMessage(HttpStatusCode.InternalServerError);
        }

        private static IEnumerable<TAttribute> GetCustomAttributesForControllerMethod<TAttribute>(ApiController controller)
            where TAttribute : Attribute
            =>
            controller?.ActionContext.ActionDescriptor.GetCustomAttributes<TAttribute>();


    }
}