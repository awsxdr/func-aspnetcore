namespace Func.AspNet
{
    using System;
    using System.Net;
    using System.Net.Http;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ProducesStatusCodeAttribute : Attribute
    {
        public HttpStatusCode StatusCode { get; }
        public string Message { get; } = string.Empty;

        internal HttpResponseMessage ToResponseMessage() =>
            new HttpResponseMessage(StatusCode) { Content = new StringContent(Message) };

        public ProducesStatusCodeAttribute(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }
    }
}
