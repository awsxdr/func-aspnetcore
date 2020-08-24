namespace Func.AspNet.Example
{
    using System.Net;

    [ProducesStatusCode(HttpStatusCode.NotFound)]
    public class NotFoundError : ResultError { }

    public class PageNotFoundError : NotFoundError { }
    public class DocumentNotFoundError : NotFoundError { }

    [ProducesStatusCode(HttpStatusCode.Unauthorized)]
    public class UnauthorizedError : ResultError { }
}
