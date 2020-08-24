namespace Func.AspNet.Example
{
    using System;
    using System.Net;

    [ProducesStatusCode(HttpStatusCode.NotFound)]
    public class NotFoundError : ResultError { }

    public class PageNotFoundError : NotFoundError { }

    [MessageTextSource(nameof(Message))]
    public class DocumentNotFoundError : NotFoundError 
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Message => $"Could not find document {Id}";
    }

    [ProducesStatusCode(HttpStatusCode.Unauthorized, Message = "Not found")]
    public class UnauthorizedError : ResultError { }
}
