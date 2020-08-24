namespace Func.AspNet
{
    using System.Net;

    public class ResponseDetails
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
    }
}
