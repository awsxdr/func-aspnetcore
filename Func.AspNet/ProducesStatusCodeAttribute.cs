namespace Func.AspNet
{
    using System;
    using System.Net;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ProducesStatusCodeAttribute : Attribute
    {
        public HttpStatusCode StatusCode { get; }
        public string Message { get; set; } = string.Empty;

        private readonly Lazy<ResponseDetails> _responseDetailsFactory;
        internal ResponseDetails ResponseDetails => _responseDetailsFactory.Value;

        public ProducesStatusCodeAttribute(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
            _responseDetailsFactory = new Lazy<ResponseDetails>(() => new ResponseDetails { Message = Message, StatusCode = StatusCode });
        }
    }
}
