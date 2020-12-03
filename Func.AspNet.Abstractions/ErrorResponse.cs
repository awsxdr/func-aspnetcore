namespace Func.AspNet
{
    using System.Net;

    public class ErrorResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public object Body { get; set; }
    }
}