namespace Func.AspNet
{
    using System;
    using System.Net;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class OnSuccessAttribute : Attribute
    {
        public HttpStatusCode StatusCode { get; }

        public OnSuccessAttribute(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }
    }
}
