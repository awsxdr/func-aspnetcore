namespace Func.AspNetCore.Example
{
    [ProducesStatusCode(404)]
    public class NotFoundError : ResultError { }

    public class PageNotFoundError : NotFoundError { }
    public class DocumentNotFoundError : NotFoundError { }

    [ProducesStatusCode(401)]
    public class UnauthorizedError : ResultError { }
}
