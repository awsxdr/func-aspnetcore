namespace Func.AspNetCore
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class ResultFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context) => 
            context.Result =
                context.Result is ObjectResult o
                && o.Value is Result r
                ? r switch
                {
                    Success s => GetSuccessResult(context, s),
                    Failure f => GetFailureResult(context, f),
                    _ => new StatusCodeResult(500)
                }
                : context.Result;

        public void OnActionExecuting(ActionExecutingContext context)
        {
        }

        private static IActionResult GetSuccessResult(ActionExecutedContext context, Success success)
        {
            IActionResult GetResultForSuccess(int statusCode) =>
                success.GetValue() switch
                {
                    Some<object> s =>  new ObjectResult(s.Value) { StatusCode = statusCode },
                    _ => new StatusCodeResult(statusCode)
                };

            return (
                GetCustomAttributesForControllerMethod<OnSuccessAttribute>(context.Controller as ControllerBase)
                .SingleOrDefault()
                ?.StatusCode
                ?? 200)
            .Map(GetResultForSuccess);
        }

        private static IActionResult GetFailureResult(ActionExecutedContext context, Failure failure)
        {
            var errorType = failure.GetError().GetType();

            return (
                GetCustomAttributesForControllerMethod<OnFailureAttribute>(context.Controller as ControllerBase)
                ?.FirstOrDefault(x => x.ErrorType.IsAssignableFrom(errorType))
                ?.CodeMessageTuple
                ?? errorType.GetCustomAttribute<ProducesStatusCodeAttribute>()?.CodeMessageTuple                
                ?? (500, ""))
            .Map(x => string.IsNullOrEmpty(x.Message) 
                ? (IActionResult) new StatusCodeResult(x.StatusCode)
                : new ObjectResult(x.Message) { StatusCode = x.StatusCode });
        }

        private static IEnumerable<TAttribute> GetCustomAttributesForControllerMethod<TAttribute>(ControllerBase controller)
            where TAttribute : Attribute
            =>
            controller?.ControllerContext.ActionDescriptor.MethodInfo.GetCustomAttributes<TAttribute>();
    }
}
