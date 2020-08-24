namespace Func.AspNet
{
    using System;
    using System.Net;
    using System.Net.Http;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public sealed class OnFailureAttribute : Attribute
    {
        public HttpStatusCode StatusCode { get; }
        public Type ErrorType { get; }
        public string Message { get; set; } = string.Empty;

        private Lazy<ResponseDetails> _responseDetailsFactory;
        internal ResponseDetails ResponseDetails => _responseDetailsFactory.Value;

        public OnFailureAttribute(Type errorType, HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
            ErrorType = errorType;
            _responseDetailsFactory = new Lazy<ResponseDetails>(() => new ResponseDetails { Message = Message, StatusCode = StatusCode });
        }
    }
}
